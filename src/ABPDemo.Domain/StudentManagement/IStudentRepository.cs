using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace ABPDemo.StudentManagement
{
    public interface IStudentRepository : IRepository<Student, Guid>
    {
        Task<int> GetStudentCountAsync(int skipCount, int maxResultCount, CancellationToken cancellationToken);

        Task<List<Student>> GetStudentListAsync(int skipCount, int maxResultCount, string sorting, CancellationToken cancellationToken);
    }
}
