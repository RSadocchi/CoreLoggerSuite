using CoreLogger.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreLogger
{
    public class InspectorService : IInspectorService
    {
        readonly IWebHostEnvironment _environment;
        readonly IMemoryCache _cache;
        readonly ICoreLogger _logger;
        readonly Credentials _credentials;
        readonly CoreLoggerConfiguration _options;
        public CoreLoggerConfiguration Options => _options;

        public InspectorService(
            IWebHostEnvironment environment,
            IMemoryCache cache,
            ICoreLogger logger)
        {
            _environment = environment ?? throw new ArgumentNullException($"Injection fail for {nameof(IWebHostEnvironment)} in CoreLogger.Inspector");
            _cache = cache ?? throw new ArgumentNullException($"Injection fail for {nameof(IMemoryCache)} in CoreLogger.Inspector");
            _logger = logger ?? throw new ArgumentNullException($"Injection fail for {nameof(ICoreLogger)} in CoreLogger.Inspector");
            _options = _logger.Configuration;
            _credentials = _logger
                .Configuration
                .Environments
                ?.Where(t => !t.DenyEnvironmet && t.EnvironmentName == _environment.EnvironmentName)
                ?.FirstOrDefault()
                ?.UseAuthentication;
        }

        #region ///Auth
        public bool IsLogged()
        {
            bool? logged = null;
            DateTime? deadline = null;
            if (_credentials != null)
                if (!_cache.TryGetValue(CacheKeys.Logged, out logged) || logged == false)
                    if (!_cache.TryGetValue(CacheKeys.LoginInstant, out deadline) ||
                        deadline.GetValueOrDefault(DateTime.MinValue).AddSeconds(_credentials.Expiring.TotalSeconds) < DateTime.Now)
                        return false;
            return true;
        }

        public (bool success, string message) Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username)) return (false, "Username cannot be empty");
            if (string.IsNullOrWhiteSpace(password)) return (false, "Password cannot be empty");
            if (username.Trim().Equals(_credentials.Username.Trim(), StringComparison.InvariantCultureIgnoreCase)
                && password.Trim().Equals(_credentials.Password.Trim()))
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions() { AbsoluteExpirationRelativeToNow = _credentials.Expiring.Add(TimeSpan.FromSeconds(30)) };
                _cache.Set(CacheKeys.Logged, true, cacheEntryOptions);
                _cache.Set(CacheKeys.LoginInstant, DateTime.Now, cacheEntryOptions);
                return (true, null);
            }
            return (false, "Invalid credentials.");
        }

        public void Logout()
        {
            _cache.Remove(CacheKeys.Logged);
            _cache.Remove(CacheKeys.LoginInstant);
        }

        public bool EnvAllowed()
        {
            if (_logger.Configuration.Environments?.Count() <= 0) return true;
            var env = _logger.Configuration.Environments.FirstOrDefault(t => t.EnvironmentName == _environment.EnvironmentName);
            if (env == null || !env.DenyEnvironmet) return true;
            return false;
        }
        #endregion

        public async Task<Log_Master> Get(LogSource source, string id, int? level = null)
        {
            var logs = await GetList(source: source, level: level, from: null, to: null);
            if (source == LogSource.File) return logs.FirstOrDefault(t => t.DateTime.Ticks.ToString() == id);
            else if (int.TryParse(id, out int _id) && _id > 0) return logs.FirstOrDefault(t => t.ID == _id);
            return null;

        }

        public async Task<IEnumerable<Log_Master>> GetList(LogSource source, int? level = null, DateTime? from = null, DateTime? to = null)
            => (await _logger.GetList(source, level, from, to)).ToList();
    }


}
