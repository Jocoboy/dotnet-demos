using ABPDemo.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPDemo.UserManagement.Dtos
{
    public class CurrentUserInfoDto : AccountSimpleDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// 用户账号类别
        /// </summary>
        public UserAccountType AccountType { get; set; }

        /// <summary>
        /// 角色列表
        /// </summary>
        public IList<string> Roles { get; set; }
    }
}
