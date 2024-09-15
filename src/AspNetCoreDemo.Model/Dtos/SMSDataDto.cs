using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.Dtos
{
    public class SMSDataDto
    {
        public string SmsId { get; set; }

        public string Phone { get; set; }

        public string Content { get; set; }

        /// <summary>
        /// 接口要传，但是为空值即可
        /// </summary>
        public string SpNumber { get; set; }
    }

    public class RegMessageDto<T> where T : class
    {
        public string ClientId { get; set; }

        public string SendTime { get; set; }

        public string SecurityKey { get; set; }

        public List<T> Data { get; set; }

    }
}
