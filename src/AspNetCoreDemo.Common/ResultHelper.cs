using AspNetCoreDemo.Common.Extensions;
using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Common
{
    /// <summary>
    /// 错误类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ResultHelper<T> where T : class
    {
        public static MessageDto<T> GetResult(ErrorEnum error, T data = null, string message = null)
        {

            return message == null ? new MessageDto<T>() { Res = (int)error, Msg = error.GetRemark(), Data = data } :
                new MessageDto<T>() { Res = (int)error, Msg = message, Data = data };
        }

        public static MessageDto<T> Success(T data = null)
        {
            return GetResult(ErrorEnum.Success, data);
        }
    }
}
