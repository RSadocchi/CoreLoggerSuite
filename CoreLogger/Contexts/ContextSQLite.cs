using CoreLogger.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoreLogger.Contexts
{
    internal class ContextSQLite : DbContext, IUnitOfWork
    {
        readonly string _dbFullPath;

        public DbSet<Log_Master> Logger_Masters { get; set; }

        public ContextSQLite(string databaseFullPath)
        {
            _dbFullPath = databaseFullPath ?? throw new ArgumentNullException("Database path cannot be null");
            this.Database.EnsureCreated();
            this.Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder bld)
        {
            base.OnConfiguring(bld);
            bld.EnableSensitiveDataLogging();
            bld.UseSqlite($"Filename={_dbFullPath}");
        }

        protected override void OnModelCreating(ModelBuilder bld)
        {
            base.OnModelCreating(bld);
        }

        public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(base.SaveChangesAsync().Result > 0);
    }
}
