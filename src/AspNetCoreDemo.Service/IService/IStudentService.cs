using AspNetCoreDemo.Model.Dtos;
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
    }
}
