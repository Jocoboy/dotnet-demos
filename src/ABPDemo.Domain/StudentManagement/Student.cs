using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp;
using ABPDemo.Enums;

namespace ABPDemo.StudentManagement
{
    public class Student : Entity<Guid>
    {
        public string Name { get; set; }
        public StudentLevelType StudentLevel { get; set; }

        public List<StudentScore> StudentScores { get; set; }
        public List<StudentCourse> StudentCourses { get; set; }
    }
}
