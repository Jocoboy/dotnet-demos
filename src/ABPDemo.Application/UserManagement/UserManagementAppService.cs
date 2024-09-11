using ABPDemo.Components;
using ABPDemo.Enums;
using ABPDemo.ExceptionHandle;
using ABPDemo.Permissions;
using ABPDemo.UserManagement.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Users;
using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace ABPDemo.UserManagement
{
    [Authorize(ABPDemoPermissions.SystemSetting)]
    public class UserManagementAppService : ABPDemoAppService, IUserManagementAppService
    {
        private readonly IExpectedExceptionCollection _expectedExceptions;
        private readonly IdentityUserManager _userManager;
        private readonly IPermissionManager _permissionManager;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly ForbiddenUserCache _forbiddenUserCache;

        public UserManagementAppService(IExpectedExceptionCollection expectedExceptions, IdentityUserManager userManager, IPermissionManager permissionManager, IIdentityUserRepository identityUserRepository, ForbiddenUserCache forbiddenUserCache)
        {
            _expectedExceptions = expectedExceptions;
            _userManager = userManager;
            _permissionManager = permissionManager;
            _identityUserRepository = identityUserRepository;
            _forbiddenUserCache = forbiddenUserCache;
        }

        public async Task<Guid> CreateAccountAsync(AccountCreateInput input, CancellationToken cancellationToken)
        {
            _expectedExceptions.Add(new ExpectedExceptionDescription() { Type = ExpectedExceptionType.UniqueViolation, Key = "UX_AbpUsers_UserName", ErrorCode = ABPDemoDomainErrorCodes.UserNameAlreadyExists, ErrorMessage = "用户名已存在!" });

            var user = new IdentityUser(GuidGenerator.Create(), input.UserName, string.Empty);
            user.Name = input.Name;
            user.SetAccountType(UserAccountType.Admin);

            (await _userManager.CreateAsync(user, input.Password)).CheckErrors();

            foreach (var code in input.PermissionCodes)
            {
                await _permissionManager.SetAsync(code, UserPermissionValueProvider.ProviderName, user.Id.ToString(), true);
            }
            return user.Id;
        }

        public async Task<List<AccountDto>> GetAccountListAsync(CancellationToken cancellationToken)
        {
            var list = await _identityUserRepository.GetListAsync(sorting: $"{nameof(IdentityUser.CreationTime)} desc", cancellationToken: cancellationToken);
            var result = ObjectMapper.Map<List<IdentityUser>, List<AccountDto>>(list);

            foreach (var user in result)
            {
                var permissions = await _permissionManager.GetAllForUserAsync(user.Id);
                user.PermissionCodes = permissions.Where(x => x.IsGranted)
                    .Select(x => x.Name).ToList();
            }
            return result;
        }

        public async Task UpdateAccountAsync(Guid userId, AccountUpdateInput input, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || user.GetAccountType() != UserAccountType.Admin)
            {
                throw new UserFriendlyException("操作用户不存在！", ABPDemoDomainErrorCodes.UserNotExist);
            }

            user.Name = input.Name;

            (await _userManager.UpdateAsync(user)).CheckErrors();

            var permissions = await _permissionManager.GetAllForUserAsync(user.Id);

            foreach (var code in input.PermissionCodes)
            {
                await _permissionManager.SetAsync(code, UserPermissionValueProvider.ProviderName, user.Id.ToString(), true);
            }

            var revokePermissions = permissions.Where(x => !input.PermissionCodes.Contains(x.Name)).ToList();

            foreach (var code in revokePermissions)
            {
                await _permissionManager.SetAsync(code.Name, UserPermissionValueProvider.ProviderName, user.Id.ToString(), false);
            }
        }

        public async Task ChangeAccountPasswordAsync(Guid userId, string password, CancellationToken cancellationToken)
        {
            var user = await _identityUserRepository.GetAsync(userId, false, cancellationToken);

            if (user.GetAccountType() != UserAccountType.Admin)
            {
                throw new ArgumentException("指定的用户不存在!", nameof(userId));
            }

            (await _userManager.RemovePasswordAsync(user)).CheckErrors();
            (await _userManager.AddPasswordAsync(user, password)).CheckErrors();
            (await _userManager.ResetAccessFailedCountAsync(user)).CheckErrors();
            (await _userManager.SetLockoutEndDateAsync(user, null)).CheckErrors();
        }

        public async Task DeleteAccountAsync(Guid userId, CancellationToken cancellationToken)
        {
            if (CurrentUser.Id == userId)
            {
                throw new UserFriendlyException("不能删除你自己的账号!", ABPDemoDomainErrorCodes.CanNotDeleteCurrentUser);
            }

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null || user.GetAccountType() != UserAccountType.Admin)
            {
                return;
            }

            (await _userManager.DeleteAsync(user)).CheckErrors();

            await _forbiddenUserCache.ForbiddenAsync(userId, cancellationToken);
        }
    }
}
