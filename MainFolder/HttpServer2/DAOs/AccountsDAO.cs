using HttpServer2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.DAOs
{
    public class AccountsDAO
    {
        public static async Task<Account?> GetAccountById(int id)
            => (await MyORM.Instance.Select<Account>())
                .FirstOrDefault(x => x.Id == id);

        public static async Task<Account?> GetAccountByLoginAndPassword(string login, string password)
        {
            var accountCandidate = await GetAccountByLogin(login);

            if (accountCandidate is null)
                return accountCandidate;

            return PasswordHashingManager.GetSHA256(password + accountCandidate.Salt) == accountCandidate.Password ?
                accountCandidate :
                null;
        }

        public static async Task<Account?> GetAccountByLogin(string login)
            => (await MyORM.Instance.Select<Account>())
                .FirstOrDefault(x => x.Login == login);
    }
}
