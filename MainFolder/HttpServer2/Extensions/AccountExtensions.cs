using HttpServer2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.Extensions
{
    public static class AccountExtensions
    {
        public static string GetAuthorName(this IEnumerable<Account> accounts, int? id)
            => accounts.FirstOrDefault(x => x.Id == id)?.Login ?? "Аноним";
    }
}
