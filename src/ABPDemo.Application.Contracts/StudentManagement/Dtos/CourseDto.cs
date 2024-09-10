using ABPDemo.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPDemo.StudentManagement.Dtos
{
    public class CourseDto
    {
        public string Name { get; set; }
        public float Credit { get; set; }
        public CourseType Type { get; set; }
    }
}
