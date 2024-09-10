using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace ABPDemo.StudentManagement
{
    public class StudentScore : Entity
    {
        public Guid StudentId { get; set; }
        public int Year { get; set; }
        public float TotalGrade { get; set; }

        public override object[] GetKeys()
        {
            return new object[] { StudentId, Year };
        }
    }
}
