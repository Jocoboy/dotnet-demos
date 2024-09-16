using AspNetCoreDemo.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.Enums
{
    public enum OprModuleType
    {
        [Remark("登录")]
        Login = 0
    }
}
