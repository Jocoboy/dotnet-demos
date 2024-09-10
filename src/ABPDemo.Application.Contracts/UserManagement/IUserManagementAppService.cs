using ABPDemo.UserManagement.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ABPDemo.UserManagement
{
    public interface IUserManagementAppService : IApplicationService
    {
        Task<Guid> CreateAccountAsync(AccountCreateInput input, CancellationToken cancellationToken);

        Task<List<AccountDto>> GetAccountListAsync(CancellationToken cancellationToken);

        Task UpdateAccountAsync(Guid userId, AccountUpdateInput input, CancellationToken cancellationToken);

        Task ChangeAccountPasswordAsync(Guid userId, string password, CancellationToken cancellationToken);

        Task DeleteAccountAsync(Guid userId, CancellationToken cancellationToken);
    }
}
