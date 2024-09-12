﻿using ABPDemo.Attributes;
using ABPDemo.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPDemo.UserManagement.Dtos
{
    public class AccountCreateInput : AccountSimpleDto
    {
        /// <summary>
        /// 用户密码
        /// </summary>
        [Required, MinLength(6), MaxLength(15)]
        public string Password { get; set; }

        /// <summary>
        /// 用户类型
        /// </summary>
        public UserAccountType UserAccountType { get; set; }
    }
}
