using ABPDemo.AdvisoryLock;
using ABPDemo.Permissions;
using ABPDemo.StudentManagement.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace ABPDemo.StudentManagement
{
    public class StudentAppService : ABPDemoAppService, IStudentAppService
    {

        private readonly IStudentRepository _studentRepository;
        private readonly IAdvisoryLock _advisoryLock;

        public StudentAppService(IStudentRepository studentRepository, IAdvisoryLock advisoryLock)
        {
            _studentRepository = studentRepository;
            _advisoryLock = advisoryLock;
        }

        public async Task<PagedResultDto<StudentDto>> GetStudentListAsync(StudentFilterInput input, CancellationToken cancellationToken)
        {
            var total = await _studentRepository.GetStudentCountAsync(input.SkipCount, input.MaxResultCount, cancellationToken);

            var list = await _studentRepository.GetStudentListAsync(input.SkipCount, input.MaxResultCount, input.Sorting, cancellationToken);

            var dtos = ObjectMapper.Map<List<Student>, List<StudentDto>>(list);

            var result = new PagedResultDto<StudentDto>(total, dtos);

            return result;
        }

        [Authorize(Roles = ABPDemoRoles.Admin)]
        public async Task<StudentSimpleDto> UpdateStudentAsync(StudentInput input, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetAsync(input.Id, false, cancellationToken);

            student.Name = input.Name;
            student.StudentLevel = input.StudentLevel;

            await _studentRepository.UpdateAsync(student, false, cancellationToken);

            return ObjectMapper.Map<Student, StudentSimpleDto>(student);
        }
    }
}
