using AspNetCoreDemo.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.Enums
{
    public enum ErrorEnum
    {
        /// <summary>
        /// 成功
        /// </summary>
        [Remark("SUCCESS")]
        Success = 0,

        /// <summary>
        /// 若错误需要详细告知可使用此错误码后返回自定义错误内容
        /// </summary>
        [Remark("数据不合法或不正确")]
        DataError = -1099,
    }
}
