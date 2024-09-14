using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.EFCore.Entity
{
    public class StudentScore
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int Year { get; set; }
        public float TotalGrade { get; set; }
    }
}
