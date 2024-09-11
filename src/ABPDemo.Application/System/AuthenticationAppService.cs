using ABPDemo.Enums;
using ABPDemo.System.Dtos;
using ABPDemo.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace ABPDemo.System
{
    public class AuthenticationAppService : ABPDemoAppService, IAuthenticationAppService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IdentitySecurityLogManager _identitySecurityLogManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AuthOptions _authOptions;
        private readonly HttpContext _context;

        public AuthenticationAppService(
            SignInManager<IdentityUser> signInManager,
            IdentitySecurityLogManager identitySecurityLogManager,
            UserManager<IdentityUser> userManager,
            IOptions<AuthOptions> options,
            IHttpContextAccessor httpContextAccessor)
        {
            _signInManager = signInManager;
            _identitySecurityLogManager = identitySecurityLogManager;
            _userManager = userManager;
            _authOptions = options.Value;
            _context = httpContextAccessor.HttpContext;
        }

        public async Task LoginAsync(LoginInput input, CancellationToken cancellationToken)
        {
            SignInResult result;
            using (var uow = UnitOfWorkManager.Begin(requiresNew: true))
            {
                result = await _signInManager.PasswordSignInAsync(input.UserName, input.Password, input.RememberMe, true);
                await _identitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                {
                    Identity = IdentitySecurityLogIdentityConsts.Identity,
                    Action = result.ToIdentitySecurityLogAction(),
                    UserName = input.UserName
                });
            }

            if (result.IsNotAllowed) throw new UserFriendlyException("该账号被禁用！");

            if (result.IsLockedOut) throw new UserFriendlyException("错误次数过多，请稍后再试！");

            if (!result.Succeeded) throw new UserFriendlyException("账号密码错误，请重新输入！");

            var user = await _userManager.FindByNameAsync(input.UserName);
            var token = await GenerateUserTokenAsync(user, input.RememberMe);

            _context.Response.Headers["Access-Control-Expose-Headers"] = "*";
            _context.Response.Headers["X-Bearer"] = token;
        }

        [Authorize]
        public async Task LoginOutAsync(CancellationToken cancellationToken)
        {
            await _signInManager.SignOutAsync();
        }

        [Authorize]
        public async Task RefreshTokenAsync(CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(CurrentUser.Id.ToString());
            var token = await GenerateUserTokenAsync(user, false);
            _context.Response.Headers["Access-Control-Expose-Headers"] = "*";
            _context.Response.Headers["X-Bearer"] = token;
        }

        private async Task<string> GenerateUserTokenAsync(IdentityUser user, bool rememberMe)
        {
            #region 额外角色信息写入token
            var accountType = user.GetAccountType();
            var accountTypeValue = (int)accountType;
            var roles = await _userManager.GetRolesAsync(user);
            roles.Clear(); // 清空ABP自己写入的Role = "admin"
            roles.Add(accountType.ToString());
            #endregion

            var firstLoginTime = CurrentUser?.FindClaimValue(ABPDemoClaimTypes.FirstLoginTime) ?? DateTime.UtcNow.ToString("o");
            var rememberMeDays = CurrentUser?.FindClaimValue(ABPDemoClaimTypes.RememberMeDays) ?? (rememberMe ? "7" : "0");
            var claims = new List<Claim> {
                new Claim(AbpClaimTypes.UserId, user.Id.ToString()),
                new Claim(ABPDemoClaimTypes.AccountType, accountTypeValue.ToString()),
                new Claim(ABPDemoClaimTypes.FirstLoginTime, firstLoginTime),
                new Claim(ABPDemoClaimTypes.RememberMeDays, rememberMeDays)
            };
            claims.AddRange(roles.Select(x => new Claim(AbpClaimTypes.Role, x)));
            _context.User = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authOptions.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_authOptions.Issuer,
                _authOptions.Audience,
                claims,
                DateTime.Now,
                DateTime.Now.AddMinutes(_authOptions.AccessExpiration),
                credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
