using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.EFCore.Entity
{
    /// <summary>
    /// 角色菜单对应表
    /// </summary>
    public class SysRole
    {
        public int Id { get; set; }
        public string RoleCode { get; set; }
        public string MenuCode { get; set; }
    }
}
