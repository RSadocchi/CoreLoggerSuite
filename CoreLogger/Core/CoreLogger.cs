using CoreLogger.Contexts;
using CoreLogger.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CoreLogger
{
    public class CoreLogger : ICoreLogger
    {
        readonly CoreLoggerConfiguration _configuration;
        public CoreLoggerConfiguration Configuration => _configuration;

        public CoreLogger(IOptions<CoreLoggerConfiguration> configuration)
        {
            _configuration = configuration.Value ?? throw new ArgumentNullException("Configurations cannot be null");

            if (!string.IsNullOrWhiteSpace(_configuration.SQL_ConnectionString))
                using (var ctx = new ContextSQL(_configuration.SQL_ConnectionString))
                {
                    ctx.Database.Migrate();
                }
        }

        public Task LogTrace(string message, string data = null, [CallerMemberName] string caller = "", [CallerLineNumber] int line = 0)
        {
            var entity = new Log_Master()
            {
                DateTime = DateTime.Now,
                LevelID = (int)LogLevel.Trace,
                CallerMemberName = caller,
                CallerMemberLineNumber = line,
                Message = message,
                FullData = data
            };

            if (_configuration.UseDailyLogFile)
                using (var ctx = new ContextFile(_configuration.File_FolderPath))
                    ctx.Log(entity);

            if (_configuration.UseSQLite)
                using (var ctx = new ContextSQLite(_configuration.SQLite_FullPath))
                {
                    ctx.Logger_Masters.Add(entity);
                    ctx.SaveEntitiesAsync();
                }

            if (_configuration.UseSQL)
                using (var ctx = new ContextSQL(_configuration.SQL_ConnectionString))
                {
                    entity.ID = 0;
                    ctx.Logger_Masters.Add(entity);
                    ctx.SaveEntitiesAsync();
                }

            return Task.CompletedTask;
        }

        public Task LogInformation(string message, string data = null, [CallerMemberName] string caller = "", [CallerLineNumber] int line = 0)
        {
            var entity = new Log_Master()
            {
                DateTime = DateTime.Now,
                LevelID = (int)LogLevel.Information,
                CallerMemberName = caller,
                CallerMemberLineNumber = line,
                Message = message,
                FullData = data
            };

            if (_configuration.UseDailyLogFile)
                using (var ctx = new ContextFile(_configuration.File_FolderPath))
                    ctx.Log(entity);

            if (_configuration.UseSQLite)
                using (var ctx = new ContextSQLite(_configuration.SQLite_FullPath))
                {
                    ctx.Logger_Masters.Add(entity);
                    ctx.SaveEntitiesAsync();
                }

            if (_configuration.UseSQL)
                using (var ctx = new ContextSQL(_configuration.SQL_ConnectionString))
                {
                    entity.ID = 0;
                    ctx.Logger_Masters.Add(entity);
                    ctx.SaveEntitiesAsync();
                }

            return Task.CompletedTask;
        }

        public Task LogWarning(string message, string data = null, [CallerMemberName] string caller = "", [CallerLineNumber] int line = 0)
        {
            var entity = new Log_Master()
            {
                DateTime = DateTime.Now,
                LevelID = (int)LogLevel.Warning,
                CallerMemberName = caller,
                CallerMemberLineNumber = line,
                Message = message,
                FullData = data
            };

            if (_configuration.UseDailyLogFile)
                using (var ctx = new ContextFile(_configuration.File_FolderPath))
                    ctx.Log(entity);

            if (_configuration.UseSQLite)
                using (var ctx = new ContextSQLite(_configuration.SQLite_FullPath))
                {
                    ctx.Logger_Masters.Add(entity);
                    ctx.SaveEntitiesAsync();
                }

            if (_configuration.UseSQL)
                using (var ctx = new ContextSQL(_configuration.SQL_ConnectionString))
                {
                    entity.ID = 0;
                    ctx.Logger_Masters.Add(entity);
                    ctx.SaveEntitiesAsync();
                }

            return Task.CompletedTask;
        }

        public Task LogError(string message, string data = null, [CallerMemberName] string caller = "", [CallerLineNumber] int line = 0)
        {
            var entity = new Log_Master()
            {
                DateTime = DateTime.Now,
                LevelID = (int)LogLevel.Error,
                CallerMemberName = caller,
                CallerMemberLineNumber = line,
                Message = message,
                FullData = data
            };

            if (_configuration.UseDailyLogFile)
                using (var ctx = new ContextFile(_configuration.File_FolderPath))
                    ctx.Log(entity);

            if (_configuration.UseSQLite)
                using (var ctx = new ContextSQLite(_configuration.SQLite_FullPath))
                {
                    ctx.Logger_Masters.Add(entity);
                    ctx.SaveEntitiesAsync();
                }

            if (_configuration.UseSQL)
                using (var ctx = new ContextSQL(_configuration.SQL_ConnectionString))
                {
                    entity.ID = 0;
                    ctx.Logger_Masters.Add(entity);
                    ctx.SaveEntitiesAsync();
                }

            return Task.CompletedTask;
        }

        public Task LogError(string message = null, Exception exception = null, [CallerMemberName] string caller = "", [CallerLineNumber] int line = 0)
        {
            var entity = new Log_Master()
            {
                DateTime = DateTime.Now,
                LevelID = (int)LogLevel.Error,
                CallerMemberName = caller,
                CallerMemberLineNumber = line,
                Message = message ?? exception.Message,
                FullData = exception.ToString()
            };

            if (_configuration.UseDailyLogFile)
                using (var ctx = new ContextFile(_configuration.File_FolderPath))
                    ctx.Log(entity);

            if (_configuration.UseSQLite)
                using (var ctx = new ContextSQLite(_configuration.SQLite_FullPath))
                {
                    ctx.Logger_Masters.Add(entity);
                    ctx.SaveEntitiesAsync();
                }

            if (_configuration.UseSQL)
                using (var ctx = new ContextSQL(_configuration.SQL_ConnectionString))
                {
                    entity.ID = 0;
                    ctx.Logger_Masters.Add(entity);
                    ctx.SaveEntitiesAsync();
                }

            return Task.CompletedTask;
        }

        public Task<IEnumerable<Log_Master>> GetList(LogSource source, int? level = null, DateTime? from = null, DateTime? to = null)
        {
            IEnumerable<Log_Master> entities = null;
            if (source == LogSource.SQLite) return SQLite_ListAsync(level, from, to);
            if (source == LogSource.SQL) return SQL_ListAsync(level, from, to);
            if (source == LogSource.File) return FileLog_ListAsync(level, from, to);
            return Task.FromResult(entities);
        }

        public Task<Log_Master> Get(LogSource source, LogLevel level, string id)
        {
            return Task.FromResult(new Log_Master());
        }

        private Task<IEnumerable<Log_Master>> SQLite_ListAsync(int? level = null, DateTime? from = null, DateTime? to = null)
        {
            IEnumerable<Log_Master> entities = null;
            if (_configuration.UseSQLite)
                using (var ctx = new ContextSQLite(_configuration.SQLite_FullPath))
                    entities = ctx.Logger_Masters
                        .Where(t => level.HasValue ? level.Value == t.LevelID : true)
                        .Where(t => from.HasValue ? from.Value.Date <= t.DateTime : true)
                        .Where(t => to.HasValue ? to.Value.AddDays(1).Date > t.DateTime : true)
                        .ToList();
            return Task.FromResult(entities);
        }

        private Task<IEnumerable<Log_Master>> SQL_ListAsync(int? level = null, DateTime? from = null, DateTime? to = null)
        {
            IEnumerable<Log_Master> entities = null;
            if (_configuration.UseSQL)
                using (var ctx = new ContextSQL(_configuration.SQL_ConnectionString))
                    entities = ctx.Logger_Masters
                        .Where(t => level.HasValue ? level.Value == t.LevelID : true)
                        .Where(t => from.HasValue ? from.Value.Date <= t.DateTime : true)
                        .Where(t => to.HasValue ? to.Value.AddDays(1).Date > t.DateTime : true)
                        .ToList();
            return Task.FromResult(entities);
        }

        private Task<IEnumerable<Log_Master>> FileLog_ListAsync(int? level = null, DateTime? from = null, DateTime? to = null)
        {
            IEnumerable<Log_Master> entities = null;
            if (_configuration.UseDailyLogFile)
                using (var ctx = new ContextFile(_configuration.File_FolderPath))
                    entities = ctx.GetList(level, from, to).Result;
            return Task.FromResult(entities);
        }
    }
}
