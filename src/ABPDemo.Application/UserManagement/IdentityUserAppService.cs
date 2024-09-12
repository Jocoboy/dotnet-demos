using ABPDemo.UserManagement.Dtos;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Users;
using Volo.Abp;
using Microsoft.AspNetCore.Identity;

namespace ABPDemo.UserManagement
{
    public class IdentityUserAppService : ABPDemoAppService, IIdentityUserAppService
    {
        private readonly IdentityUserManager _userManager;
        private readonly IPermissionManager _permissionManager;


        public IdentityUserAppService(IdentityUserManager userManager,
            IPermissionManager permissionManager
            )
        {
            _userManager = userManager;
            _permissionManager = permissionManager;
        }

        [Authorize]
        public async Task CheckPasswordAsync(string password)
        {
            var user = await _userManager.FindByIdAsync(CurrentUser.Id.ToString());

            var result = await _userManager.CheckPasswordAsync(user, password);

            if (!result) throw new UserFriendlyException("输入密码不正确!", ABPDemoDomainErrorCodes.IncorrectPassword);
        }

        [Authorize]
        public async Task<CurrentUserInfoDto> GetCurrentUserInfoAsync()
        {
            var user = await _userManager.FindByIdAsync(CurrentUser.Id.ToString());

            var roles = await _userManager.GetRolesAsync(user);

            var permissions = await _permissionManager.GetAllForUserAsync(user.Id);

            var dto = new CurrentUserInfoDto()
            {
                UserId = user.Id,
                UserName = user.UserName,
                Name = user.Name,
                AccountType = user.GetAccountType(),
                Roles = roles,
                PermissionCodes = permissions.Where(x => x.IsGranted)
                .Select(x => x.Name).ToList()
            };

            return dto;
        }

        [Authorize]
        public async Task UpdatePasswordAsync(UpdatePasswordInput input)
        {
            var user = await _userManager.FindByIdAsync(CurrentUser.Id.ToString());

            (await _userManager.ChangePasswordAsync(user, input.OldPassword, input.NewPassword)).CheckErrors();
        }
    }
}
