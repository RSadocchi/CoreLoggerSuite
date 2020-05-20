using CoreLogger.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoreLogger.Contexts
{
    internal class ContextSQL : DbContext, IUnitOfWork
    {
        public string ConnectionString { get; set; }
        readonly string _connectionString;

        public DbSet<Log_Master> Logger_Masters { get; set; }

        public ContextSQL() { }

        public ContextSQL(string connectionString) : base()
        {
            _connectionString = connectionString ?? throw new ArgumentNullException("Connection string cannot be null");
        }

        public ContextSQL(DbContextOptions<DbContext> options, string connectionString) : base(options)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException("Connection string cannot be null");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder bld)
        {
            base.OnConfiguring(bld);
            bld.EnableSensitiveDataLogging();
            bld.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder bld)
        {
            base.OnModelCreating(bld);
            bld.Entity<Log_Master>(e =>
            {
                e.ToTable(nameof(Log_Master), "Log");
            });
        }

        public Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(base.SaveChangesAsync().Result > 0);
    }
}
