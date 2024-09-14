using AspNetCoreDemo.Common.Extensions;
using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Repository.IRepository;
using AspNetCoreDemo.Repository.Repository.Base;
using AspNetCoreDemo.Repository.Uow;
using MySql.Data.MySqlClient;

namespace AspNetCoreDemo.Repository.Repository
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        public StudentRepository(IUnitofWork unitOfWork) : base(unitOfWork)
        {
        }

        public IEnumerable<StudentDataDto> GetStudentData(out long count, StudentSearchDto dto, bool isAll = false)
        {
            MySqlParameter[] parameters = new MySqlParameter[]
           {
                new MySqlParameter("@PageStart", (dto.Page-1)*dto.PageSize),
                new MySqlParameter("@PageEnd", dto.PageSize),
                new MySqlParameter("@Name", "%"+dto.Name+"%"),
                new MySqlParameter("@StudentLevel", dto.StudentLevel)
           };

            string countsql = @"SELECT COUNT(s.Id) FROM student s";
            string sql = @"SELECT Name, StudentLevel FROM student s";
            string where = " WHERE 1 = 1";

            if (!string.IsNullOrEmpty(dto.Name))
            {
                where += " AND Name LIKE @Name";
            }
            if (!string.IsNullOrEmpty(Convert.ToString(dto.StudentLevel)))
            {
                where += " AND StudentLevel = @StudentLevel";
            }

            count = Convert.ToInt64(_context.Database.SqlQueryCount(countsql + where, parameters));

            if (!isAll)
            {
                where += " LIMIT @PageStart, @PageEnd";
            }

            return _context.Database.SqlQuery<StudentDataDto>(sql + where, parameters);
        }

    }
}
