using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Repository.IRepository.Base;

namespace AspNetCoreDemo.Repository.IRepository
{
    public interface IStudentRepository : IBaseRepository<Student>
    {
         IEnumerable<StudentDataDto> GetStudentData(out long count, StudentSearchDto dto, bool isAll = false);
    }
}
