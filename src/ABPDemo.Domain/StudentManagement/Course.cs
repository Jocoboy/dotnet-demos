using ABPDemo.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace ABPDemo.StudentManagement
{
    public class Course : Entity<Guid>
    {
        public string Name { get; set; }
        public float Credit { get; set; }
        public CourseType Type { get; set; }
    }
}
