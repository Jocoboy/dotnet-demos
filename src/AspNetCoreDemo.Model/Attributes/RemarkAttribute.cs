using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.Attributes
{
    /// <summary>
    /// 设置字段备注值
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property,//可以应用于字段|属性
       Inherited = true,//允许继承
       AllowMultiple = false)]//同一字段不允许运用多次
    public class RemarkAttribute : Attribute
    {
        public string Remark { get; set; }

        public RemarkAttribute(string remark)
        {
            Remark = remark;
        }
    }
}
