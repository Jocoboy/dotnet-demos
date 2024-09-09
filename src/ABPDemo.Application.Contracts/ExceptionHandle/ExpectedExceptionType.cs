using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPDemo.ExceptionHandle
{
    public enum ExpectedExceptionType
    {
        /// <summary>
        /// 唯一性约束异常
        /// </summary>
        UniqueViolation,

        /// <summary>
        /// 外键约束异常
        /// </summary>
        ForeignKeyViolation
    }
}
