using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoreLogger.Contexts
{
    internal interface IUnitOfWork : IDisposable
    {
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
    }
}
