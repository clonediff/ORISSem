using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer2.ServerInfrstructure.CookiesAndSessions
{
    public class SessionManager
    {
        readonly MemoryCache _chache = new MemoryCache(new MemoryCacheOptions());

        private static Lazy<SessionManager> _sessionManager = new(() => new SessionManager());

        public static SessionManager Inst => _sessionManager.Value;

        public Session CreateSession(int accountId, string login, DateTime createdTime = default)
        {
            var guid = Guid.NewGuid();
            if (createdTime == default)
                createdTime = DateTime.Now;
            var session = new Session { Id = guid, AccountId = accountId, Login = login, CreateDateTime = createdTime };
            _chache.Set(guid, session,
                new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(2)));
            return session;
        }

        public bool CheckSession(Guid id)
            => _chache.TryGetValue<Session>(id, out var session);

        public Session GetSessionInfo(Guid id)
            => _chache.TryGetValue<Session>(id, out var session) ?
            session! :
            throw new ArgumentException($"Не найдено сессии с ключом {id}");
    }
}
