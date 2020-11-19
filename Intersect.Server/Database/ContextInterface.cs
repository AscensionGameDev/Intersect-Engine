using Microsoft.EntityFrameworkCore.Infrastructure;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Intersect.Server.Database
{
    public abstract class ContextInterface<TContext> : IDisposable where TContext : IDbContext
    {
        private static readonly object Lock = new object();

        protected ContextInterface(TContext context)
        {
            Context = context;
            Monitor.Enter(Lock);
        }

        public TContext Context { get; }

        #region Implementation of IDbContext

        /// <inheritdoc />
        public DatabaseFacade Database => Context.Database;

        /// <inheritdoc />
        public int SaveChanges() => Context.SaveChanges();

        /// <inheritdoc />
        public int SaveChanges(bool acceptAllChangesOnSuccess) => Context.SaveChanges(acceptAllChangesOnSuccess);

        /// <inheritdoc />
        public Task<int>
            SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default) =>
            Context.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        /// <inheritdoc />
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            Context.SaveChangesAsync(cancellationToken);

        #endregion

        #region IDisposable

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.SaveChanges();
                Monitor.Exit(Lock);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable
    }
}
