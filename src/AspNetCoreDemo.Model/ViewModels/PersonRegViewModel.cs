using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.ViewModels
{
    public class PersonRegViewModel
    {
        [Required(ErrorMessage = "手机号不能为空")]
        [MaxLength(11, ErrorMessage = "长度不可大于11位")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        [MinLength(6, ErrorMessage = "密码最少为6位")]
        [MaxLength(20, ErrorMessage = "密码最多为20位")]
        public string Pwd { get; set; }

        [Required(ErrorMessage = "验证码不能为空")]
        public string Code { get; set; }
    }
}
