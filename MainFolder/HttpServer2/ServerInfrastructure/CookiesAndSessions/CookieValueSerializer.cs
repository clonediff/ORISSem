using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HttpServer2.ServerInfrstructure.CookiesAndSessions
{
    public static class CookieValueSerializer
    {
        public static string Serialize(ICookieValue cookie)
        {
            var result = new List<string>();
            foreach (var property in cookie.GetType()
                .GetProperties()
                .Where(x => !Attribute.IsDefined(x, typeof(JsonIgnoreAttribute))))
                result.Add($"\"{property.Name}\":{JsonSerializer.Serialize(property.GetValue(cookie))}");
            return $"{{{string.Join(" ", result)}}}";
        }

        public static T Deserialize<T>(string value) => (T)Deserialize(value, typeof(T));

        public static object Deserialize(string value, Type type)
        {
            if (value[0] != '{' ||
                value[^1] != '}')
                throw new ArgumentException($"Невозможно преобразовать {value} в объект типа {type}");
            value = value.Substring(1, value.Length - 2);
            var result = Activator.CreateInstance(type);
            foreach (var property in value.Split())
            {
                var keyValue = property.Split(':');
                var propName = keyValue[0].Trim('"');
                var propValue = keyValue[1];

                var objectProp = type.GetProperties().FirstOrDefault(x => !Attribute.IsDefined(x, typeof(JsonIgnoreAttribute)) && x.Name == propName) ??
                    throw new ArgumentException($"У типа {type} нет доступного свойства {propName}");
                objectProp.SetValue(result, JsonSerializer.Deserialize(propValue, objectProp.PropertyType));
            }
            return result;
        }
    }
}
