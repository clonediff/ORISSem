using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.ServerInfrstructure.CookiesAndSessions
{
    public class Session
    {
        public Guid Id { get; set; }
        public int AccountId { get; set; }
        public string Login { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
}
