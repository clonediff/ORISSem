using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.Extensions
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Abs(this TimeSpan value)
        {
            if (value < TimeSpan.Zero)
                return value * (-1);
            return value;
        }
    }
}
