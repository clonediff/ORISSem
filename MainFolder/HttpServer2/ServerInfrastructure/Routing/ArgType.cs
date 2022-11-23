using System;
using System.Collections.Generic;
using System.Linq;

namespace HttpServer2.Routing
{
    public class ArgType
    {
        public static IList<ArgType> argTypes = new List<ArgType>()
        {
            new ArgType(typeof(string), s => s),
            new ArgType(typeof(Guid), s => Guid.Parse(s)),
            new ArgType(typeof(double), s => double.Parse(s)),
            new ArgType(typeof(int), s => int.Parse(s)),
            new ArgType(typeof(bool), s => bool.Parse(s))
        };

        public static bool TryParse<T>(string strValue, out T val)
        {
            try
            {
                val = (T)GetArgType(typeof(T)).Parser(strValue);
                return true;
            } catch
            {
                val = default!;
                return false;
            }
        }
        public static bool TryParse(string strValue, out object val, out Type type)
        {
            for (int i = argTypes.Count - 1; i >= 0; i--)
            {
                var argType = argTypes[i];
                try
                {
                    val = argType.Parser(strValue);
                    type = argType.Type;
                    return true;
                }
                catch { continue; }
            }

            val = default!;
            type = default!;
            return false;
        }

        public static ArgType? GetArgType(Type type)
            => argTypes.FirstOrDefault(x => x.Type == type);

        public Type Type { get; }
        public Func<string, object> Parser { get; }

        private ArgType(Type type, Func<string, object> parser)
        {
            Type = type;
            Parser = parser;
        }
    }
}
