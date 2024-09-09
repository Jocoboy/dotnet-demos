using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPDemo.ExceptionHandle
{
    public class ExpectedExceptionDescription
    {
        public ExpectedExceptionDescription()
        {
        }

        public ExpectedExceptionDescription(ExpectedExceptionType type, string key, string errorCode, string errorMessage)
        {
            Type = type;
            Key = key;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// 异常类型 
        /// </summary>
        public ExpectedExceptionType Type { get; set; }

        /// <summary>
        /// 异常标识
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 异常代码
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// 异常描述
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
