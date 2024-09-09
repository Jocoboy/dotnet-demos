using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPDemo.Attributes
{
    public class StringTrimAttribute : Attribute
    {
        private string[] Propertys;

        public StringTrimAttribute(params string[] Propertys)
        {
            this.Propertys = Propertys;
        }
    }
}
