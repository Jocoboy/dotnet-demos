using AspNetCoreDemo.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Common.Extensions.Excel
{
    /// <summary>
    /// Excel 扩展
    /// </summary>
    public class ExcelExtension
    {
        /// <summary>
        /// 获取要导出的字段信息 (注: 这里的字段，指数据库对应字段，程序里对应的是类的属性)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<ExcelFieldInfo> GetOutputFields(Type type)
        {
            PropertyInfo[] props = type.GetProperties();
            ExcelAttribute[] excelAttrs = new ExcelAttribute[props.Length];
            for (int i = 0; i < props.Length; i++)
            {
                var excelAttr = props[i].GetCustomAttribute<ExcelAttribute>(true);
                excelAttrs[i] = excelAttr;
            }

            var fieldInfos = new List<ExcelFieldInfo>();
            for (int i = 0; i < props.Length; i++)
            {
                if (excelAttrs[i]?.IsIgnore == true)
                {
                    continue;
                }

                var fieldInfo = new ExcelFieldInfo();
                fieldInfo.Name = props[i].Name;
                fieldInfo.Title = excelAttrs[i]?.Title ?? props[i].Name;
                fieldInfo.IsDynamic = excelAttrs[i]?.IfAllNullIgnore == true;
                fieldInfos.Add(fieldInfo);
            }

            return fieldInfos;
        }

        /// <summary>
        /// Excel导出字段信息
        /// </summary>
        public class ExcelFieldInfo
        {
            /// <summary>
            /// 字段名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 是否动态字段
            /// </summary>
            public bool IsDynamic { get; set; }
        }
    }
}
