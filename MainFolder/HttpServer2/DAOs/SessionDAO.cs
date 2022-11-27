using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HttpServer2.ServerInfrstructure.CookiesAndSessions;
using HttpServer2.Extensions;

namespace HttpServer2.DAOs
{
    public class SessionDAO
    {
        private string connectionString;
        static Dictionary<Type, string> tableNames;
        static Dictionary<PropertyInfo, string> columnNames;
        static Dictionary<(Type table, string column), PropertyInfo> propertyByColumnName;

        static SessionDAO()
        {
            tableNames = new();
            columnNames = new();
            propertyByColumnName = new();

            var tableName = typeof(Session)
                .GetCustomAttributes<Table>(false)
                .FirstOrDefault(new Table(nameof(Session) + "s"));
            tableNames[typeof(Session)] = tableName.Name;

            var properties = typeof(Session).GetProperties();
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

        private SessionDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private static Lazy<SessionDAO> inst;

        public static void Init(string connectionString)
            => inst = new Lazy<SessionDAO>(() => new SessionDAO(connectionString));

        public static SessionDAO Instance => inst.Value;

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

        public async Task<int> Insert(Session obj)
        {
            var properties = typeof(Session).GetProperties();
            var propertyNamesInTable = properties.Select(x => columnNames[x]);
            var query = $"INSERT INTO {tableNames[typeof(Session)]} ({string.Join(", ", propertyNamesInTable)})\n" +
                $"VALUES ({string.Join(", ", properties.Select(x => x.GetValue(obj).GetSqlStringValue()))})";

            return await ExecuteNonQuery(query);
        }

        public async Task<int> Update(Session obj)
        {
            var properties = typeof(Session).GetProperties().Where(x => x.Name != "AccountId");
            var propertyNamesInTable = properties.Select(x => columnNames[x]);
            var query = $"UPDATE {tableNames[typeof(Session)]}\n" +
            $"SET {string.Join(", ", properties.Zip(propertyNamesInTable).Select(x => $"{x.Second} = {x.First.GetValue(obj).GetSqlStringValue()}"))}\n" +
            $"WHERE AccountId = '{obj.AccountId}'";

            return await ExecuteNonQuery(query);
        }

        public async Task<int> Delete(Session obj)
        {
            var properties = typeof(Session).GetProperties();
            var propertyNamesInTable = properties.Select(x => columnNames[x]);
            var query = $"DELETE FROM {tableNames[typeof(Session)]}\n" +
            $"WHERE {string.Join(" AND ", properties.Zip(propertyNamesInTable).Select(x => $"{x.Second} = {x.First.GetValue(obj).GetSqlStringValue()}"))}";

            return await ExecuteNonQuery(query);
        }

        public async Task<IEnumerable<Session>> Select()
        {
            var result = new List<Session>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string selectCmd = $"SELECT * FROM {tableNames[typeof(Session)]}";
                var cmd = new SqlCommand(selectCmd, connection);
                var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    var properties = typeof(Session).GetProperties();
                    var propertyNamesInTable = properties.Select(x => columnNames[x]);

                    while (reader.Read())
                    {
                        var objT = Activator.CreateInstance(typeof(Session));
                        foreach (var property in properties.Zip(propertyNamesInTable))
                        {
                            var value = reader.GetValue(reader.GetOrdinal(property.Second));
                            if (value is DBNull)
                                value = null;
                            property.First.SetValue(objT, value);
                        }
                        result.Add((Session)objT);
                    }
                }
            }

            return result;
        }

        public async Task<Session?> GetSessionById(Guid id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string selectCmd = $"SELECT * FROM {tableNames[typeof(Session)]} WHERE Id = '{id}'";
                var cmd = new SqlCommand(selectCmd, connection);
                var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    var properties = typeof(Session).GetProperties();
                    var propertyNamesInTable = properties.Select(x => columnNames[x]);

                    reader.Read();
                    var objT = Activator.CreateInstance(typeof(Session));
                    foreach (var property in properties.Zip(propertyNamesInTable))
                    {
                        var value = reader.GetValue(reader.GetOrdinal(property.Second));
                        if (value is DBNull)
                            value = null;
                        property.First.SetValue(objT, value);
                    }
                    return (Session?)objT;
                }
            }

            return null;
        }

        public async Task<Session?> GetSessionByAccountId(int accountId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string selectCmd = $"SELECT * FROM {tableNames[typeof(Session)]} WHERE AccountId = '{accountId}'";
                var cmd = new SqlCommand(selectCmd, connection);
                var reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    var properties = typeof(Session).GetProperties();
                    var propertyNamesInTable = properties.Select(x => columnNames[x]);

                    reader.Read();
                    var objT = Activator.CreateInstance(typeof(Session));
                    foreach (var property in properties.Zip(propertyNamesInTable))
                    {
                        var value = reader.GetValue(reader.GetOrdinal(property.Second));
                        if (value is DBNull)
                            value = null;
                        property.First.SetValue(objT, value);
                    }
                    return (Session)objT;
                }
            }

            return null;
        }
    }
}
