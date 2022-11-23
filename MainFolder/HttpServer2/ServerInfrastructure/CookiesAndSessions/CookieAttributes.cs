using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.ServerInfrstructure.CookiesAndSessions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CheckCookie : Attribute
    {
        public Type Type { get; }

        public string PropertyName { get; }
        public object? Value { get; }

        public CheckCookie(Type type)
        {
            if (!typeof(ICookieValue).IsAssignableFrom(type))
                throw new ArgumentException($"CheckValue must contains only ICookieValue type for checking: {type} isn't ICookieValue");
            Type = type;
        }

        public CheckCookie(Type type, string propertyName, object? value)
            : this(type)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("PropertyName can't be empty");
            PropertyName = propertyName;
            Value = value;
        }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class FromCookie : Attribute
    {
        public Type Type { get; }
        public string PropertyName { get; }

        public FromCookie(Type type)
        {
            if (!typeof(ICookieValue).IsAssignableFrom(type))
                throw new ArgumentException($"FromCookie must contains only ICookieValue type for checking: {type} isn't ICookieValue");
            Type = type;
        }

        public FromCookie(Type type, string propertyName)
            : this(type)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("PropertyName can't be empty");
            PropertyName = propertyName;
        }
    }
}
