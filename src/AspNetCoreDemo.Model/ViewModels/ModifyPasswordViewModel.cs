using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.ViewModels
{
    public class ModifyPasswordViewModel
    {
        public string OgPwd { get; set; }

        [MinLength(6, ErrorMessage = "密码最少为6位")]
        [MaxLength(20, ErrorMessage = "密码最多为20位")]
        public string NewPwd { get; set; }

        [MinLength(6, ErrorMessage = "密码最少为6位")]
        [MaxLength(20, ErrorMessage = "密码最多为20位")]
        public string ReNewPwd { get; set; }
    }
}
