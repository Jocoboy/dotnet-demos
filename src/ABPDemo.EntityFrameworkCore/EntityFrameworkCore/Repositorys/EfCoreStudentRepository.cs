using ABPDemo.StudentManagement;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace ABPDemo.EntityFrameworkCore.Repositorys
{
    public class EfCoreStudentRepository : EfCoreRepository<ABPDemoDbContext, Student, Guid>, IStudentRepository
    {
        public EfCoreStudentRepository(IDbContextProvider<ABPDemoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }

        public async Task<int> GetStudentCountAsync(int skipCount, int maxResultCount, CancellationToken cancellationToken)
        {
            var dbSet = await GetDbSetAsync();

            var result = await dbSet.CountAsync(cancellationToken);

            return result;
        }

        public async Task<List<Student>> GetStudentListAsync(int skipCount, int maxResultCount, string sorting, CancellationToken cancellationToken)
        {
            var dbSet = await GetDbSetAsync();
            
            var result = await dbSet.Include(x => x.StudentScores)
                                                .Include(x => x.StudentCourses)
                                                .ThenInclude(x => x.Course)
                                                .OrderBy(sorting.IsNullOrWhiteSpace() ? nameof(Student.Name) : sorting)
                                                .PageBy(skipCount, maxResultCount)
                                                .ToListAsync(cancellationToken);

            return result;
        }
    }
}
