using AspNetCoreDemo.Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Model.Dtos
{
    public class StudentUpdateDto
    {
        public int Id { get; set; }

        [StringLength(20, ErrorMessage = "学生姓名字符长度超过限制")]
        public string Name { get; set; }
        public StudentLevelType StudentLevel { get; set; }
    }
}
