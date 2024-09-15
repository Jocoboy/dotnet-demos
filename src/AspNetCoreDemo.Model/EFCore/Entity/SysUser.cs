using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.EFCore.Entity
{
    public class SysUser
    {
        public int Id { get; set; }
        public string RoleCode { get; set; }
        public string UserLgnId { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public bool IsLock { get; set; }
        public string Remark { get; set; }
        public int? LoginNum { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? RegDate { get; set; }
    }
}
