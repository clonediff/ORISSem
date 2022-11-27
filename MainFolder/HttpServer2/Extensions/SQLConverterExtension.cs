using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.Extensions
{
    public static  class SQLConverterExtension
    {
        public static string GetSqlStringValue(this object obj)
        {
            if (obj is null)
                return "null";
            if (obj is bool)
                return $"'{obj.ToString()!.ToLower()}'";
            if (obj is DateTime date)
                return $"'{date.ToString("yyyy-MM-dd HH:mm:ss.fff")}'";
            if (obj is Enum)
                return $"'{(int)obj}'";
            return $"'{obj}'";
        }
    }
}
