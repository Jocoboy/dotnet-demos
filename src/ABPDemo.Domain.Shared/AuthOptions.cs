using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPDemo
{
    public class AuthOptions
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// 颁发者
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 接受者
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public double AccessExpiration { get; set; }
    }
}
