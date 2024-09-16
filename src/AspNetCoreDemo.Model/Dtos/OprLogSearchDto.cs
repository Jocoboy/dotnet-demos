using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.Dtos
{
    public class OprLogSearchDto : PageDto
    {
        public string OprRole { get; set; }
        public string OprName { get; set; }
        /// <summary>
        /// 操作开始时段
        /// </summary>
        public DateTime? OprDateBegin { get; set; }
        /// <summary>
        /// 操作结束时段
        /// </summary>
        public DateTime? OprDateEnd { get; set; }
        public string OprModule { get; set; }
        public string OprRemark { get; set; }
    }
}
