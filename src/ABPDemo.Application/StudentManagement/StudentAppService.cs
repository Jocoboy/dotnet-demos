using ABPDemo.AdvisoryLock;
using ABPDemo.Enums;
using ABPDemo.Permissions;
using ABPDemo.StudentManagement.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using Volo.Abp.DistributedLocking;

namespace ABPDemo.StudentManagement
{
    public class StudentAppService : ABPDemoAppService, IStudentAppService
    {

        private readonly IStudentRepository _studentRepository;
        private readonly IAdvisoryLock _advisoryLock;
        private readonly IAbpDistributedLock _abpDistributedLock;
        private readonly IDistributedCache<StudentCacheItem> _distributeCache;

        public StudentAppService(IStudentRepository studentRepository, IAdvisoryLock advisoryLock, IAbpDistributedLock abpDistributedLock, IDistributedCache<StudentCacheItem> distributedCache)
        {
            _studentRepository = studentRepository;
            _advisoryLock = advisoryLock;
            _abpDistributedLock = abpDistributedLock;
            _distributeCache = distributedCache;
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

            await _advisoryLock.LockAsync(Locks.StudentUpdate, LockMode.Exclusive, true, cancellationToken);

            student.Name = input.Name;
            student.StudentLevel = input.StudentLevel;

            await _studentRepository.UpdateAsync(student, false, cancellationToken);

            return ObjectMapper.Map<Student, StudentSimpleDto>(student);
        }

        [Authorize(Roles = ABPDemoRoles.Admin)]
        public async Task UpdateStudentLevelWithLockAsync(Guid id, StudentLevelType level, CancellationToken cancellationToken)
        {
            // 定义锁的名称
            var lockName = $"Student:{id}:UpdateLock";
            // 尝试获取锁
            await using var handle = await _abpDistributedLock.TryAcquireAsync(lockName, TimeSpan.Zero, cancellationToken);
            if (handle != null)
            {
                // 临界区代码
                var student = await _studentRepository.GetAsync(id, false, cancellationToken);
                student.StudentLevel = level;
                await _studentRepository.UpdateAsync(student, true, cancellationToken);
            }
        }

        [Authorize(Roles = ABPDemoRoles.Admin)]
        public async Task<StudentCacheItem> GetStudentFromCacheAsync(Guid id, CancellationToken cancellationToken)
        {
            var key = $"Student:{id}"; // 缓存Key
            return await _distributeCache.GetOrAddAsync(
                key, 
                async () =>  // 如果缓存不存在，从数据库加载
                {
                    var student = await _studentRepository.GetAsync(id, false, cancellationToken);
                    var studentCacheItem = ObjectMapper.Map<Student, StudentCacheItem> (student);
                    return studentCacheItem;
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1) // 1小时后过期
                },
                null, false, cancellationToken
            );
        }
    }
}
