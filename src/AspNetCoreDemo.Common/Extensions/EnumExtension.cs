using AspNetCoreDemo.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Common.Extensions
{
    public static class EnumExtension
    {
        public static string GetRemark(this Enum enumValue)
        {
            Type type = enumValue.GetType();
            FieldInfo field = type.GetField(enumValue.ToString());
            if (field.IsDefined(typeof(RemarkAttribute), true))
            {
                return field.GetCustomAttribute<RemarkAttribute>()?.Remark;
            }
            return enumValue.ToString();
        }
    }
}
