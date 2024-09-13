using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Common.Extensions.JWT
{
    /// <summary>
    /// JWT Token配置项
    /// </summary>
    public class JWTTokenOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 加密Key
        /// </summary>
        public string SecurityKey { get; set; }
    }
}
