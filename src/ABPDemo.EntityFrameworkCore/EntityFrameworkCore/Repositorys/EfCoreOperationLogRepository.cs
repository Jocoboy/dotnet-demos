using ABPDemo.Enums;
using ABPDemo.StudentManagement;
using ABPDemo.SystemManagement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ABPDemo.EntityFrameworkCore.Repositorys
{
    public class EfCoreOperationLogRepository : EfCoreRepository<ABPDemoDbContext, OperationLog, Guid>, IOperationLogRepository
    {
        public EfCoreOperationLogRepository(IDbContextProvider<ABPDemoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<int> CountAsync(DateTime? startTime = null, DateTime? endTime = null, string accountName = null, string account = null, UserAccountType? accountType = null, OperationLogType? operation = null, CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            var result = await dbSet.WhereIf(startTime is not null, x => x.CreationTime > startTime)
                                                 .WhereIf(endTime is not null, x => x.CreationTime < endTime)
                                                 .WhereIf(accountType is not null, x => x.AccountType == accountType)
                                                 .WhereIf(operation is not null, x => x.Operation == operation)
                                                 .WhereIf(!string.IsNullOrWhiteSpace(accountName), x => x.AccountName.Contains(accountName))
                                                 .WhereIf(!string.IsNullOrWhiteSpace(account), x => x.Account.Contains(account))
                                                 .CountAsync(cancellationToken);

            return result;
        }

        public async Task<List<OperationLog>> GetListAsync(string sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, DateTime? startTime = null, DateTime? endTime = null, string accountName = null, string account = null, UserAccountType? accountType = null, OperationLogType? operation = null, CancellationToken cancellationToken = default)
        {
            var dbSet = await GetDbSetAsync();

            var result = await dbSet.WhereIf(startTime is not null, x => x.CreationTime > startTime)
                                                .WhereIf(endTime is not null, x => x.CreationTime < endTime)
                                                .WhereIf(accountType is not null, x => x.AccountType == accountType)
                                                .WhereIf(operation is not null, x => x.Operation == operation)
                                                .WhereIf(!string.IsNullOrWhiteSpace(accountName), x => x.AccountName.Contains(accountName))
                                                .WhereIf(!string.IsNullOrWhiteSpace(account), x => x.Account.Contains(account))
                                                .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(OperationLog.CreationTime) : sorting)
                                                .PageBy(skipCount, maxResultCount)
                                                .ToListAsync(cancellationToken);


            return result;
        }
    }
}
