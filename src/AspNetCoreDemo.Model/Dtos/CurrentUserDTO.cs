using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.Dtos
{
    public class CurrentUserDto
    {
        /// <summary>
        /// 登录用户Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户角色
        /// </summary>
        public string RoleCode { get; set; }
        /// <summary>
        /// 登录账号
        /// </summary>
        public string UserLgnId { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
    }
}
