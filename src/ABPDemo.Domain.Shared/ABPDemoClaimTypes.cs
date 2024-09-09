using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPDemo
{
    public class ABPDemoClaimTypes
    {
        /// <summary>
        /// 账号类型
        /// </summary>
        public static string AccountType { get; set; } = "type";

        /// <summary>
        /// 第一次登录时间
        /// </summary>
        public static string FirstLoginTime { get; set; } = "firstLoginTime";

        /// <summary>
        /// 记住我天数
        /// </summary>
        public static string RememberMeDays { get; set; } = "rememberMeDays";
    }
}
