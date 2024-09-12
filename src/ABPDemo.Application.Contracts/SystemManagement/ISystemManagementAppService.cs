using ABPDemo.SystemManagement.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace ABPDemo.SystemManagement
{
    public interface  ISystemManagementAppService
    {
        Task<PagedResultDto<OperationLogDto>> GetOperateLogListAsync(OperationLogFliterInput input, CancellationToken cancellationToken);

        Task UpdateSystemSettingsAsync(SystemSettingInput input);
    }
}
