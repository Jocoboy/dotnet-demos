using ABPDemo.Auth;
using ABPDemo.UserManagement;
using ABPDemo.UserManagement.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;

namespace ABPDemo.Controllers
{
    [RemoteService]
    [Route("api/user-management")]
    public class UserManagementController : ABPDemoController, IUserManagementAppService
    {
        private readonly IUserManagementAppService _userManagementAppService; 

       public UserManagementController(IUserManagementAppService userManagementAppService)
        {
            _userManagementAppService = userManagementAppService;
        }

        /// <summary>
        /// 创建用户账号
        /// </summary>
        [HttpPost("users")]
        public async Task<Guid> CreateAccountAsync(AccountCreateInput input, CancellationToken cancellationToken)
        {
            return await _userManagementAppService.CreateAccountAsync(input, cancellationToken);
        }

        /// <summary>
        /// 获取用户账号列表
        /// </summary>
        [HttpGet("users")]
        public async Task<List<AccountDto>> GetAccountListAsync(CancellationToken cancellationToken)
        {
            return await _userManagementAppService.GetAccountListAsync(cancellationToken);
        }

        /// <summary>
        /// 更新用户账号信息
        /// </summary>
        [HttpPut("users/{userId}")]
        public async Task UpdateAccountAsync(Guid userId, AccountUpdateInput input, CancellationToken cancellationToken)
        {
           await _userManagementAppService.UpdateAccountAsync(userId, input, cancellationToken);
        }

        /// <summary>
        /// 修改用户账号密码
        /// </summary>
        [HttpPut("users/{userId}/password")]
        public async Task ChangeAccountPasswordAsync(Guid userId, string password, CancellationToken cancellationToken)
        {
           await _userManagementAppService.ChangeAccountPasswordAsync(userId, password, cancellationToken);
        }

        /// <summary>
        /// 删除用户账号
        /// </summary>
        [HttpDelete("users/{userId}")]
        public async Task DeleteAccountAsync(Guid userId, CancellationToken cancellationToken)
        {
            await _userManagementAppService.DeleteAccountAsync(userId, cancellationToken);
        }

    }
}
