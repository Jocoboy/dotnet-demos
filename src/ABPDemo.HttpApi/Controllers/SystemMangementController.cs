using ABPDemo.Attributes;
using ABPDemo.SystemManagement.Dtos;
using ABPDemo.SystemManagement;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp;

namespace ABPDemo.Controllers
{
    [RemoteService]
    [Route("api/system-management")]
    public class SystemManagementController : ABPDemoController, ISystemManagementAppService
    {
        private readonly ISystemManagementAppService _systemManagementAppService;
        public SystemManagementController(ISystemManagementAppService systemManagementAppService)
        {
            _systemManagementAppService = systemManagementAppService;
        }

        /// <summary>
        /// 获取操作日志列表
        /// </summary>
        [HttpGet("operation-logs")]
        public Task<PagedResultDto<OperationLogDto>> GetOperateLogListAsync([StringTrim] OperationLogFliterInput input, CancellationToken cancellationToken)
        {
            return _systemManagementAppService.GetOperateLogListAsync(input, cancellationToken);
        }
    }
}
