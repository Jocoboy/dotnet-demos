using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.Dtos
{
    public class StudentDataDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public StudentLevelType StudentLevel { get; set; }
    }
}
