using HttpServer2.Attributes;
using HttpServer2.DAOs;
using HttpServer2.Dto;
using HttpServer2.Models;
using HttpServer2.ServerInfrastructure.CookiesAndSessions;
using HttpServer2.ServerInfrstructure.CookiesAndSessions;
using HttpServer2.ServerResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.Controllers
{
    [ApiController]
    public class Register
    {
        [HttpGET("/")]
        public IControllerResult RegisterGet(
            [FromCookie(typeof(SessionId), nameof(SessionId.Id))] Guid sessionId)
        {
            if (sessionId != default)
                return new Redirect("/profile");
            return new View("register", new LoginRegisterResultDto());
        }

        [HttpPOST("/")]
        public async Task<IControllerResult> RegisterPost(string login, string password,
            string repeat_password, string email,
            [FromCookie(typeof(SessionId), nameof(SessionId.Id))] Guid sessionId)
        {
            if (sessionId != default)
                return new Redirect("/profile");
            if (!InputFieldsValidator.ValidateLoginString(login))
                return new View("register",
                    new LoginRegisterResultDto { ErrorFieldName = "login", ErrorMsg = "Недопустимый формат у Логина" });
            if (!InputFieldsValidator.ValidatePasswordString(password, out var errMsg))
                return new View("register",
                    new LoginRegisterResultDto { ErrorFieldName = "password", ErrorMsg = errMsg });
            if (!InputFieldsValidator.ValidateEmailString(email))
                return new View("register",
                    new LoginRegisterResultDto { ErrorFieldName = "email", ErrorMsg = "Недопустимый формат у email" });
            if (password != repeat_password)
                return new View("register",
                    new LoginRegisterResultDto { ErrorFieldName = "repeat_password", ErrorMsg = "Повторный пароль несовпал с исходным" });

            var account = await AccountsDAO.GetAccountByLogin(login);
            if (account is not null)
                return new View("register",
                    new LoginRegisterResultDto { ErrorFieldName = "login", ErrorMsg = "Пользователь с таким логином уже существует" });

            var salt = PasswordHashingManager.CreateSalt();
            var encryptedPassword = PasswordHashingManager.GetSHA256(password + salt);

            var insertAccount = new Account { Login = login, Password = encryptedPassword, Email = email, Salt = salt };
            await MyORM.Instance.Insert(insertAccount);
            return new Redirect("/login");
        }
    }
}
