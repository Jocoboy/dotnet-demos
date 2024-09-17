using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Model.Enums;
using AspNetCoreDemo.Service.IService.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Service.IService
{
    public interface IStudentService: IBaseService<Student>
    {
        List<StudentDataDto> GetStudentData(out long count, StudentSearchDto dto, bool isAll = false);
        MessageDto<string> ImportStudentData(List<Student> students, ImportType importType);
    }
}
