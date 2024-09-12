using ABPDemo.SystemManagement.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace ABPDemo.SystemManagement
{
    public class SystemManagementAppService : ABPDemoAppService, ISystemManagementAppService
    {
        private readonly IOperationLogRepository _operateLogRepo;

        public SystemManagementAppService(IOperationLogRepository operateLogRepo)
        {
            _operateLogRepo = operateLogRepo;
        }

        public async Task<PagedResultDto<OperationLogDto>> GetOperateLogListAsync(OperationLogFliterInput input, CancellationToken cancellationToken)
        {
            var total= await _operateLogRepo.CountAsync(input.StartTime, input.EndTime, input.AccountName, input.Account, input.AccountType, input.Operation, cancellationToken);
           
            var list = await _operateLogRepo.GetListAsync(input.Sorting, input.MaxResultCount, input.SkipCount, input.StartTime, input.EndTime, input.AccountName, input.Account, input.AccountType, input.Operation, cancellationToken);
            
            var dtos = ObjectMapper.Map<List<OperationLog>, List<OperationLogDto>>(list);
            
            var result = new PagedResultDto<OperationLogDto>(total, dtos);
            
            return result;
        }
    }
}
