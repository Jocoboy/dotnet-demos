using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.Dtos
{
    public class MessageDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int Res { get; set; } = 0;

        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; } = "SUCCESS";
    }

    public class MessageDto<T> : MessageDto
    {
        /// <summary>
        /// 返回数据集合
        /// </summary>
        public T Data { get; set; }
    }
}
