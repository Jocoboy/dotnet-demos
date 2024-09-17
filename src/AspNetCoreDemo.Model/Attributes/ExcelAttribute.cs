using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.Attributes
{
    /// <summary>
    /// Excel 自定义特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class ExcelAttribute : Attribute
    {
        public ExcelAttribute() { }
        public ExcelAttribute(string title)
        {
            Title = title;
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 是否忽略当前字段，为 true 时，生成的 Excel 文件中不包含当前字段
        /// </summary>
        public bool IsIgnore { get; set; } = false;

        /// <summary>
        /// 是否在当前列全为 Null 时忽略当前字段
        /// </summary>
        public bool IfAllNullIgnore { get; set; } = false;

        /// <summary>
        /// 时间格式，DateTime 类型字段的输出格式
        /// </summary>
        public string TimeFormat { get; set; }
    }
}
