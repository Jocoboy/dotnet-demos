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
        public string Audience { get; set; }

        public string Issuer { get; set; }

        public string SecurityKey { get; set; }

        public int AccessExpiration { get; set; }
    }
}
