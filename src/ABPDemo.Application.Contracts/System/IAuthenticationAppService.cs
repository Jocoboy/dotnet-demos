using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ABPDemo.System.Dtos;
using Volo.Abp.Application.Services;

namespace ABPDemo.System
{
    public interface IAuthenticationAppService : IApplicationService
    {
        /// <summary>
        /// 登录
        /// </summary>
        Task LoginAsync(LoginInput input, CancellationToken cancellationToken);

        /// <summary>
        /// 登出
        /// </summary>
        Task LoginOutAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 刷新Token
        /// </summary>
        Task RefreshTokenAsync(CancellationToken cancellationToken);
    }
}
