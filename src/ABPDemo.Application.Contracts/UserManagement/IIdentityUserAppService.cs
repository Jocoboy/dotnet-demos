using ABPDemo.UserManagement.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPDemo.UserManagement
{
    public interface IIdentityUserAppService
    {
        Task UpdatePasswordAsync(UpdatePasswordInput input);

        Task<CurrentUserInfoDto> GetCurrentUserInfoAsync();

        Task CheckPasswordAsync(string password);
    }
}
