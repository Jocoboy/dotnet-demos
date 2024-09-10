using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace ABPDemo.StudentManagement
{
    public class StudentCourse : Entity, IHasCreationTime
    {
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public DateTime CreationTime { get; set; }

        public Student Student { get; set; }

        public Course Course { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { StudentId, CourseId };
        }
    }
}
