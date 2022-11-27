using HttpServer2.Extensions;
using HttpServer2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HttpServer2
{
    public class MyORM
    {
        private string connectionString;
        static Dictionary<Type, string> tableNames;
        static Dictionary<PropertyInfo, string> columnNames;
        static Dictionary<(Type table, string column), PropertyInfo> propertyByColumnName;

        static MyORM()
        {
            tableNames = new();
            columnNames = new();
            propertyByColumnName = new();

            var types = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.GetInterfaces().Any(i => i.Equals(typeof(IModel))));

            foreach (var type in types)
            {
                var tableName = type
                    .GetCustomAttributes<Table>(false)
                    .FirstOrDefault(new Table(type.Name + "s"));
                tableNames[type] = tableName.Name;
            }

            var properties = types.SelectMany(x => x.GetProperties());
            foreach (var property in properties)
            {
                var columnName = property
                    .GetCustomAttributes<Column>(false)
                    .FirstOrDefault(new Column(property.Name));
                columnNames[property] = columnName.Name;
                var key = (property.DeclaringType, columnName.Name);
                if (propertyByColumnName.ContainsKey(key))
                    throw new InvalidConstraintException("Внутри одной модели свойства должны иметь разные названия для столбцов");
                propertyByColumnName[key] = property;
            }
        }

        private MyORM(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private static Lazy<MyORM> inst;

        public static void Init(string connectionString)
            => inst = new Lazy<MyORM>(() => new MyORM(connectionString));

        public static MyORM Instance => inst.Value;

        public async Task<int> ExecuteNonQuery(string query)
        {
            int affectedRows = default;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var cmd = new SqlCommand(query, connection);
                try
                {
                    affectedRows = await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            return affectedRows;
        }

        public async Task<int> Insert<T>(T obj)
            where T : IModel
        {
            var properties = typeof(T).GetProperties().Where(x => x.Name != "Id");
            var propertyNamesInTable = properties.Select(x => columnNames[x]);
            var query = $"INSERT INTO {tableNames[typeof(T)]} ({string.Join(", ", propertyNamesInTable)})\n" +
                $"VALUES ({string.Join(", ", properties.Select(x => x.GetValue(obj).GetSqlStringValue()))})";

            return await ExecuteNonQuery(query);
        }
        public async Task<int> Update<T>(T obj)
            where T : IModel
        {
            var properties = typeof(T).GetProperties().Where(x => x.Name != "Id");
            var propertyNamesInTable = properties.Select(x => columnNames[x]);
            var query = $"UPDATE {tableNames[typeof(T)]}\n" +
            $"SET {string.Join(", ", properties.Zip(propertyNamesInTable).Select(x => $"{x.Second} = {x.First.GetValue(obj).GetSqlStringValue()}"))}\n" +
            $"WHERE Id = {obj.Id}";

            return await ExecuteNonQuery(query);
        }

        public async Task<int> Delete<T>(T obj)
            where T : IModel
        {
            var properties = typeof(T).GetProperties();
            var propertyNamesInTable = properties.Select(x => columnNames[x]);
            var query = $"DELETE FROM {tableNames[typeof(T)]}\n" +
            $"WHERE {string.Join(" AND ", properties.Zip(propertyNamesInTable).Select(x => $"{x.Second} = {x.First.GetValue(obj).GetSqlStringValue()}"))}";

            return await ExecuteNonQuery(query);
        }

        public async Task<IEnumerable<T>> Select<T>()
            where T : IModel
        {
            var result = new List<T>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string selectCmd = $"SELECT * FROM {tableNames[typeof(T)]}";
                var cmd = new SqlCommand(selectCmd, connection);
                var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    var properties = typeof(T).GetProperties();
                    var propertyNamesInTable = properties.Select(x => columnNames[x]);

                    while (reader.Read())
                    {
                        var objT = Activator.CreateInstance(typeof(T));
                        foreach (var property in properties.Zip(propertyNamesInTable))
                        {
                            var value = reader.GetValue(reader.GetOrdinal(property.Second));
                            if (value is DBNull)
                                value = null;
                            property.First.SetValue(objT, value);
                        }
                        result.Add((T)objT);
                    }
                }
            }

            return result;
        }
    }
}
