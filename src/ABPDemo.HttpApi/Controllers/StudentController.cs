using ABPDemo.Enums;
using ABPDemo.StudentManagement;
using ABPDemo.StudentManagement.Dtos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;

namespace ABPDemo.Controllers
{
    [RemoteService]
    [Route("api/student")]
    public class StudentController : ABPDemoController, IStudentAppService
    {
        private readonly IStudentAppService _studentAppService;

        public StudentController(IStudentAppService studentAppService)
        {
            _studentAppService = studentAppService;
        }

        /// <summary>
        /// 获取学生详细信息分页列表
        /// </summary>
        [HttpGet("list")]
        public async Task<PagedResultDto<StudentDto>> GetStudentListAsync(StudentFilterInput input, CancellationToken cancellationToken)
        {
            return await _studentAppService.GetStudentListAsync(input, cancellationToken);
        }

        /// <summary>
        /// 更新学生信息
        /// </summary>
        [HttpPost("info")]
        public async Task<StudentSimpleDto> UpdateStudentAsync(StudentInput input, CancellationToken cancellationToken)
        {
            return await _studentAppService.UpdateStudentAsync(input, cancellationToken);
        }


        /// <summary>
        /// 更新学生年级(使用Redis分布式锁)
        /// </summary>
        [HttpPost("info/level")]
        public async Task UpdateStudentLevelWithLockAsync(Guid id, StudentLevelType level, CancellationToken cancellationToken)
        {
            await _studentAppService.UpdateStudentLevelWithLockAsync(id, level, cancellationToken);
        }

        /// <summary>
        /// 获取学生信息(使用Redis分布式缓存)
        /// </summary>
        [HttpGet("info")]
        public async Task<StudentCacheItem> GetStudentFromCacheAsync(Guid id, CancellationToken cancellationToken)
        {
           return  await _studentAppService.GetStudentFromCacheAsync(id, cancellationToken);
        }
    }
}
