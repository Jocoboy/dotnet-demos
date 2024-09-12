using ABPDemo.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace ABPDemo.SystemManagement
{
    public interface IOperationLogRepository : IRepository<OperationLog, Guid>
    {
        Task<int> CountAsync(
            DateTime? startTime = null,
            DateTime? endTime = null,
            string accountName = null,
            string account = null,
            UserAccountType? accountType = null,
            OperationLogType? operation = null,
            CancellationToken cancellationToken = default);

        Task<List<OperationLog>> GetListAsync(
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            DateTime? startTime = null,
            DateTime? endTime = null,
            string accountName = null,
            string account = null,
            UserAccountType? accountType = null,
            OperationLogType? operation = null,
            CancellationToken cancellationToken = default);
    }
}
