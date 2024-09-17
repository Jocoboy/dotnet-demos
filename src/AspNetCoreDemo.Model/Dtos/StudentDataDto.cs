using AspNetCoreDemo.Model.Attributes;
using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.Dtos
{
    public class StudentDataDto
    {
        [Excel(IsIgnore = true)]
        public int Id { get; set; }
        [Excel("姓名")]
        public string Name { get; set; }
        [Excel("年级")]
        public int StudentLevel { get; set; }
    }
}
