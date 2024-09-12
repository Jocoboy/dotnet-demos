﻿using ABPDemo.Authentication;
using ABPDemo.Authentication.Dtos;
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
    [Route("api/auth")]
    public class AuthenticationController : ABPDemoController, IAuthenticationAppService
    {
        private readonly IAuthenticationAppService _authenticationAppService;

        public AuthenticationController(IAuthenticationAppService service)
        {
            _authenticationAppService = service;
        }

        /// <summary>
        /// 登录
        /// </summary>
        [HttpPost("login")]
        public async Task<string> LoginAsync(LoginInput input, CancellationToken cancellationToken)
        {
           return await _authenticationAppService.LoginAsync(input, cancellationToken);
        }

        /// <summary>
        /// 登出
        /// </summary>
        [HttpPost("logout")]
        public async Task LoginOutAsync(CancellationToken cancellationToken)
        {
            await _authenticationAppService.LoginOutAsync(cancellationToken);
        }

        /// <summary>
        /// 刷新Token
        /// </summary>z
        [HttpPost("refresh-token")]
        public async Task<string> RefreshTokenAsync(CancellationToken cancellationToken)
        {
            return await _authenticationAppService.RefreshTokenAsync(cancellationToken);
        }
    }
}
