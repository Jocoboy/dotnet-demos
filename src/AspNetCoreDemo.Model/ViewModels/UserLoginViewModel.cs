using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.ViewModels
{
    public class UserLoginViewModel
    {
        [Required(ErrorMessage = "账号不能为空")]
        public string UserLgnId { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        public string UserPwd { get; set; }

        [Required(ErrorMessage = "验证码不能为空")]
        public string Code { get; set; }
    }
}
