using ABPDemo.Enums;
using ABPDemo.StudentManagement.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace ABPDemo.StudentManagement
{
    public interface IStudentAppService : IApplicationService
    {
        Task<PagedResultDto<StudentDto>> GetStudentListAsync(StudentFilterInput input, CancellationToken cancellationToken);

        Task<StudentSimpleDto> UpdateStudentAsync(StudentInput input, CancellationToken cancellationToken);

        Task UpdateStudentLevelWithLockAsync(Guid id, StudentLevelType level, CancellationToken cancellationToken);

        Task<StudentCacheItem> GetStudentFromCacheAsync(Guid id, CancellationToken cancellationToken);
    }
}
