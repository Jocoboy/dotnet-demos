using ABPDemo.Permissions;
using ABPDemo.RedisCache;
using ABPDemo.Settings;
using ABPDemo.SystemManagement.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.SettingManagement;

namespace ABPDemo.SystemManagement
{
    public class SystemManagementAppService : ABPDemoAppService, ISystemManagementAppService
    {
        private readonly IOperationLogRepository _operateLogRepo;
        private readonly ISettingManager _settingManager;
        private readonly RedisCacheManager _redisCacheManager;

        public SystemManagementAppService(IOperationLogRepository operateLogRepo, ISettingManager settingManager, RedisCacheManager redisCacheManager)
        {
            _operateLogRepo = operateLogRepo;
            _settingManager = settingManager;
            _redisCacheManager = redisCacheManager;
        }

        public async Task<PagedResultDto<OperationLogDto>> GetOperateLogListAsync(OperationLogFliterInput input, CancellationToken cancellationToken)
        {
            var total= await _operateLogRepo.CountAsync(input.StartTime, input.EndTime, input.AccountName, input.Account, input.AccountType, input.Operation, cancellationToken);
           
            var list = await _operateLogRepo.GetListAsync(input.Sorting, input.MaxResultCount, input.SkipCount, input.StartTime, input.EndTime, input.AccountName, input.Account, input.AccountType, input.Operation, cancellationToken);
            
            var dtos = ObjectMapper.Map<List<OperationLog>, List<OperationLogDto>>(list);
            
            var result = new PagedResultDto<OperationLogDto>(total, dtos);
            
            return result;
        }

        [Authorize(ABPDemoPermissions.SystemSetting)]
        public async Task UpdateSystemSettingsAsync(SystemSettingInput input)
        {
            await _settingManager.SetGlobalAsync(ABPDemoSettings.ResetPassword, input.ResetPassword);
        }

        public async Task ClearRedisCacheAsync(string keyPrefix)
        {
            await _redisCacheManager.ClearByPrefixAsync(keyPrefix);
        }
    }
}
