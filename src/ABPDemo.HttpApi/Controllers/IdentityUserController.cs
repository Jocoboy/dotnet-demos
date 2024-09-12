using ABPDemo.UserManagement;
using ABPDemo.UserManagement.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp;

namespace ABPDemo.Controllers
{
    [RemoteService]
    [Route("api/identity")]
    public class IdentityUserController : ABPDemoController, IIdentityUserAppService
    {
        private readonly IIdentityUserAppService _identityUserAppService;
        public IdentityUserController(IIdentityUserAppService identityUserAppService)
        {
            _identityUserAppService = identityUserAppService;
        }

        /// <summary>
        /// 当前登录用户修改密码
        /// </summary>
        [HttpPut("current/password")]
        public async Task UpdatePasswordAsync([Required] UpdatePasswordInput input)
        {
            await _identityUserAppService.UpdatePasswordAsync(input);
        }

        /// <summary>
        /// 获取当前登录用户信息
        /// </summary>
        [HttpGet("current")]
        public async Task<CurrentUserInfoDto> GetCurrentUserInfoAsync()
        {
            return await _identityUserAppService.GetCurrentUserInfoAsync();
        }

        /// <summary>
        /// 校验当前登录人密码是否正确
        /// </summary>
        [HttpPost("current/check-password")]
        public async Task CheckPasswordAsync([FromBody] string password)
        {
            await _identityUserAppService.CheckPasswordAsync(password);
        }
    }
}
