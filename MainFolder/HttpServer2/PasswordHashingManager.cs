using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2
{
    public class PasswordHashingManager
    {
        public static string CreateSalt() => Guid.NewGuid().ToString();

        public static string GetSHA256(string saltedPassword)
        {
            var inputBytes = Encoding.UTF8.GetBytes(saltedPassword);
            using var sha256 = SHA256.Create();

            var outputBytes = sha256.ComputeHash(inputBytes);

            return string.Join("", outputBytes.Select(b => b.ToString()));
        }
    }
}
