using ABPDemo.Auth;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ABPDemo.Controllers
{
    [Route("api/auth")]
    public class AuthenticationController : ABPDemoController, IAuthenticationAppService
    {
        private readonly IAuthenticationAppService _service;

        public AuthenticationController(IAuthenticationAppService service)
        {
            _service = service;
        }

        /// <summary>
        /// 登录
        /// </summary>
        [HttpPost("login")]
        public async Task LoginAsync(LoginInput input, CancellationToken cancellationToken)
        {
            await _service.LoginAsync(input, cancellationToken);
        }

        /// <summary>
        /// 登出
        /// </summary>
        [HttpPost("logout")]
        public async Task LoginOutAsync(CancellationToken cancellationToken)
        {
            await _service.LoginOutAsync(cancellationToken);
        }

        /// <summary>
        /// 刷新Token
        /// </summary>z
        [HttpPost("refresh-token")]
        public async Task RefreshTokenAsync(CancellationToken cancellationToken)
        {
            await _service.RefreshTokenAsync(cancellationToken);
        }
    }
}
