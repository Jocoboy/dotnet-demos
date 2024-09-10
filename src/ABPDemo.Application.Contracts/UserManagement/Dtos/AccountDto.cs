using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPDemo.UserManagement.Dtos
{
    public class AccountDto : AccountSimpleDto
    {
        public Guid Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// 账号活动状态
        /// </summary>
        public bool IsActive { get; set; }
    }

    public class AccountSimpleDto
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 权限代码集合
        /// </summary>
        public List<string> PermissionCodes { get; set; }
    }
}
