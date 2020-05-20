using CoreLogger.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreLogger
{
    public interface IInspectorService
    {
        CoreLoggerConfiguration Options { get; }

        bool IsLogged();
        (bool success, string message) Login(string username, string password);
        void Logout();
        bool EnvAllowed();
        
        Task<IEnumerable<Log_Master>> GetList(LogSource source, int? level = null, DateTime? from = null, DateTime? to = null);
        Task<Log_Master> Get(LogSource source, string id, int? level = null);
    }
}