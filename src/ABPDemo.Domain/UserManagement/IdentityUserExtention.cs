using ABPDemo.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.Identity;

namespace ABPDemo.UserManagement
{
    public static class IdentityUserExtention
    {
        public static UserAccountType GetAccountType(this IdentityUser user)
        {
            return user.GetProperty<UserAccountType>("Type");
        }
        public static void SetAccountType(this IdentityUser user, UserAccountType accountType)
        {
            user.SetProperty("Type", accountType);
        }
    }
}
