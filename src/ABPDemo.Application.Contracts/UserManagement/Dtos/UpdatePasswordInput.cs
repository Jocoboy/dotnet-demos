using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPDemo.UserManagement.Dtos
{
    public class UpdatePasswordInput
    {
        /// <summary>
        /// 旧密码
        /// </summary>
        [Required, MinLength(6), MaxLength(15)]
        public string OldPassword { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Required, MinLength(6), MaxLength(15)]
        public string NewPassword { get; set; }
    }
}
