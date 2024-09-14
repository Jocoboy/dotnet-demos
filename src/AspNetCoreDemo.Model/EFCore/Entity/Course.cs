using AspNetCoreDemo.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.EFCore.Entity
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Credit { get; set; }
        public CourseType Type { get; set; }
    }
}
