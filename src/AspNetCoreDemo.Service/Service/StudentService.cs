using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Repository.IRepository;
using AspNetCoreDemo.Repository.IRepository.Base;
using AspNetCoreDemo.Service.IService;
using AspNetCoreDemo.Service.Service.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Service.Service
{
    public class StudentService : BaseService<Student>, IStudentService
    {
        readonly IStudentRepository _student;

        public StudentService(IStudentRepository student)
        {
            _student = student;
        }

        public List<StudentDataDto> GetStudentData(out long count, StudentSearchDto dto, bool isAll = false)
        {
            var list = _student.GetStudentData(out count, dto, isAll).ToList();

            return list;
        }
    }
}
