using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string Code { get; set; }

        public string Phone { get; set; }
        [MinLength(6, ErrorMessage = "密码最少为6位")]
        [MaxLength(20, ErrorMessage = "密码最多为20位")]
        public string Pwd { get; set; }
        [MinLength(6, ErrorMessage = "密码最少为6位")]
        [MaxLength(20, ErrorMessage = "密码最多为20位")]
        public string RePwd { get; set; }
    }
}
