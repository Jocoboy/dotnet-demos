using AspNetCoreDemo.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.Dtos
{
    public class StudentSearchDto :  PageDto
    {
        public string Name { get; set; }
        public StudentLevelType? StudentLevel { get; set; }
    }
}
