using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreDemo.Model.Attributes;
using AspNetCoreDemo.Model.Enums;

namespace AspNetCoreDemo.Model.EFCore.Entity
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public StudentLevelType StudentLevel { get; set; }
    }
}
