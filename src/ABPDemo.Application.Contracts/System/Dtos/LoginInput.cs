using ABPDemo.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPDemo.System.Dtos
{
    public class LoginInput
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Required, MinLength(5), MaxLength(11), StringTrim]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required, MinLength(6), MaxLength(15), StringTrim]
        public string Password { get; set; }

        /// <summary>
        /// 记住
        /// </summary>
        public bool RememberMe { get; set; }
    }
}
