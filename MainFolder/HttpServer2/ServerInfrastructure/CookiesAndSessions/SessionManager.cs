using HttpServer2.DAOs;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.ServerInfrstructure.CookiesAndSessions
{
    public class SessionManager
    {
        private static Lazy<SessionManager> _sessionManager = new(() => new SessionManager());

        public static SessionManager Inst => _sessionManager.Value;

        public async Task<Session> CreateSession(int accountId, string login, TimeSpan expiresIn, DateTime createdTime = default)
        {
            return await CreateSession(accountId, login, false, expiresIn, createdTime);
        }

        private async Task<Session> CreateSession(int accountId, string login, bool unlimited, TimeSpan? expiresIn, DateTime createdTime)
        {
            var foundSessionTask = SessionDAO.Instance.GetSessionByAccountId(accountId);
            var guid = Guid.NewGuid();
            while(await SessionDAO.Instance.GetSessionById(guid) is not null)
                guid = Guid.NewGuid();
            if (createdTime == default)
                createdTime = DateTime.Now;
            var session = new Session
            {
                Id = guid,
                AccountId = accountId,
                Login = login,
                CreateDateTime = createdTime,
                Unlimited = unlimited,
                Expires = createdTime + expiresIn
            };
            var foundSession = await foundSessionTask;
            if (foundSession is null)
            {
                if (unlimited || session.Expires > session.CreateDateTime)
                    await SessionDAO.Instance.Insert(session);
            }
            else
            {
                if (unlimited || session.Expires > session.CreateDateTime)
                    await SessionDAO.Instance.Update(session);
                else 
                    await SessionDAO.Instance.Delete(foundSession);
            }
            return session;
        }

        public async Task<Session> CreateUnlimitedSession(int accountId, string login, DateTime createdTime = default)
        {
            return await CreateSession(accountId, login, true, null, createdTime);
        }

        public async Task<bool> CheckSession(Guid id)
            => (await GetSessionInfo(id)) is not null;

        public async Task<Session?> GetSessionInfo(Guid id)
        {
            var session = await SessionDAO.Instance.GetSessionById(id);
            if (session is null)
                return null;
            if (session.Expires < DateTime.Now)
            {
                await SessionDAO.Instance.Delete(session);
                return null;
            }
            return session;
        }
    }
}
