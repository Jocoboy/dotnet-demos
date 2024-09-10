using ABPDemo.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPDemo.StudentManagement.Dtos
{
    public class StudentDto : StudentSimpleDto
    {
        public List<StudentScoreDto> StudentScores { get; set; }

        public List<CourseDto> Courses { get; set; }
    }

    public class StudentSimpleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public StudentLevelType StudentLevel { get; set; }
    }
}
