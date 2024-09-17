using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.EFCore.Entity
{
    /// <summary>
    /// 后台二级菜单
    /// </summary>
    public class SysMenu
    {
        /// <summary>
        /// 菜单Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 菜单层次码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 菜单地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        ///一级菜单标识 
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int Seq { get; set; }
    }
}
