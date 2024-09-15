using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.EFCore.Entity
{
    public class SMSInfo
    {
        public int Id { get; set; }

        public int? PersonId { get; set; }

        public DateTime? PassDate { get; set; }

        public DateTime? SendDate { get; set; }

        public string Phone { get; set; }

        public string Message { get; set; }

        public string SendStatus { get; set; }

        /// <summary>
        /// 回执，到达状态
        /// </summary>
        public string ArrStatus { get; set; }

        public string Guid { get; set; }

        public string SendUser { get; set; }
    }
}
