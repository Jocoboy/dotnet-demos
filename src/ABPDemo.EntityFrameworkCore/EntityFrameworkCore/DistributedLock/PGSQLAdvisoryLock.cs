using ABPDemo.AdvisoryLock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;

namespace ABPDemo.EntityFrameworkCore.DistributedLock
{
    public class PGSQLAdvisoryLock : IAdvisoryLock, ISingletonDependency
    {
        IDbContextProvider<ABPDemoDbContext> _dbContextProvider;

        public PGSQLAdvisoryLock(IDbContextProvider<ABPDemoDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public async Task<ISessionLock> LockAsync(long k, LockMode lockMode, bool waiting, CancellationToken cancellationToken = default)
        {
            var database = (await _dbContextProvider.GetDbContextAsync()).Database;

            var locked = await InternalLockAsync(new object[] { k }, lockMode, waiting, false, database, cancellationToken);

            ISessionLock result = locked ? new PGSQLSessionLock(database, k, lockMode == LockMode.Shared) : null;

            return result;
        }

        public async Task<ISessionLock> LockAsync(int m, int n, LockMode lockMode, bool waiting, CancellationToken cancellationToken = default)
        {
            var database = (await _dbContextProvider.GetDbContextAsync()).Database;

            var locked = await InternalLockAsync(new object[] { m, n }, lockMode, waiting, false, database, cancellationToken);

            ISessionLock result = locked ? new PGSQLSessionLock(database, m, n, lockMode == LockMode.Shared) : null;

            return result;
        }

        public async Task<bool> XactLockAsync(long k, LockMode lockMode, bool waiting, CancellationToken cancellationToken = default)
        {
            var database = (await _dbContextProvider.GetDbContextAsync()).Database;

            return await InternalLockAsync(new object[] { k }, lockMode, waiting, true, database, cancellationToken);
        }

        public async Task<bool> XactLockAsync(int m, int n, LockMode lockMode, bool waiting, CancellationToken cancellationToken = default)
        {
            var database = (await _dbContextProvider.GetDbContextAsync()).Database;

            return await InternalLockAsync(new object[] { m, n }, lockMode, waiting, true, database, cancellationToken);
        }
        private async Task<bool> InternalLockAsync(object[] parameters, LockMode lockMode, bool waiting, bool isXact, DatabaseFacade database, CancellationToken cancellationToken)
        {
            bool locked;

            var xact = isXact ? "_xact" : "";

            var param = parameters.Length == 1 ? "{0}" : "{0},{1}";

            var mode = lockMode switch
            {
                LockMode.Exclusive => "",
                LockMode.Shared => "_shared",
                _ => throw new NotImplementedException($"LockMode.{lockMode} is not implemented.")
            };

            if (waiting)
            {
                await database.ExecuteSqlRawAsync($"select pg_advisory{xact}_lock{mode}({param})", parameters);

                locked = true;
            }
            else
            {
                locked = (await database.SqlQueryRaw<bool>($"select pg_try_advisory{xact}_lock{mode}({param})", parameters).ToListAsync(cancellationToken)).Single();
            }
            return locked;
        }
    }
}
