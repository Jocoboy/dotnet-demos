using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.Dtos
{
    /// <summary>
    /// 一级菜单
    /// </summary>
    public class FirstMenuDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }


        /// <summary>
        /// 二级菜单
        /// </summary>
        public List<SecondMenuDto> SecondMenus { get; set; }
    }

    /// <summary>
    /// 2级菜单
    /// </summary>
    public class SecondMenuDto
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Url { get; set; }
    }
}
