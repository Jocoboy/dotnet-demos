using ABPDemo.Attributes;
using ABPDemo.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace ABPDemo.SystemManagement.Dtos
{
    public class OperationLogFliterInput : PagedAndSortedResultRequestDto
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 账号名称
        /// </summary>
        [StringTrim]
        public string AccountName { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [StringTrim]
        public string Account { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [StringTrim]
        public UserAccountType? AccountType { get; set; }

        /// <summary>
        /// 操作行为
        /// </summary>
        public OperationLogType? Operation { get; set; }
    }
}
