using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.EFCore.Entity
{
    public class OprLog
    {
        public int Id { get; set; }

        /// <summary>
        /// 操作人ID
        /// </summary>
        public int OprId { get; set; }

        /// <summary>
        /// 操作人角色
        /// </summary>
        public string OprRole { get; set; }

        /// <summary>
        /// 前台用户填手机号，后台用户填用户名(昵称)
        /// </summary>
        public string OprName { get; set; }

        public DateTime OprDate { get; set; }

        public string IP { get; set; }

        /// <summary>
        /// 操作模块
        /// </summary>
        public string OprModule { get; set; }

        /// <summary>
        /// 具体操作记录
        /// </summary>
        public string OprRemark { get; set; }
    }
}
