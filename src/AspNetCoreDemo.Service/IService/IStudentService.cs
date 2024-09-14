using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.EFCore.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Service.IService
{
    public interface IStudentService
    {
        List<StudentDataDto> GetStudentData(out long count, StudentSearchDto dto, bool isAll = false);

        Student GetStudentById(int id);

        void UpdateStudent(Student student);
    }
}
