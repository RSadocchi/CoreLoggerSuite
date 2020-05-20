using CoreLogger.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CoreLogger
{
    public interface ICoreLogger
    {
        CoreLoggerConfiguration Configuration { get; }

        Task<Log_Master> Get(LogSource source, LogLevel level, string id);
        Task<IEnumerable<Log_Master>> GetList(LogSource source, int? level = null, DateTime? from = null, DateTime? to = null);
        Task LogError(string message = null, Exception exception = null, [CallerMemberName] string caller = "", [CallerLineNumber] int line = 0);
        Task LogError(string message, string data = null, [CallerMemberName] string caller = "", [CallerLineNumber] int line = 0);
        Task LogInformation(string message, string data = null, [CallerMemberName] string caller = "", [CallerLineNumber] int line = 0);
        Task LogTrace(string message, string data = null, [CallerMemberName] string caller = "", [CallerLineNumber] int line = 0);
        Task LogWarning(string message, string data = null, [CallerMemberName] string caller = "", [CallerLineNumber] int line = 0);
    }
}