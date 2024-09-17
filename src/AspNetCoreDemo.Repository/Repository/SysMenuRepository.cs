using AspNetCoreDemo.Common.Extensions;
using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Repository.IRepository;
using AspNetCoreDemo.Repository.Repository.Base;
using AspNetCoreDemo.Repository.Uow;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Repository.Repository
{
    public class SysMenuRepository : BaseRepository<SysMenu>, ISysMenuRepository
    {
        public SysMenuRepository(IUnitofWork unitOfWork) : base(unitOfWork)
        {
        }

        public IEnumerable<SysMenu> GetSysUserMenuByRole(string role)
        {
            MySqlParameter[] parameter = new MySqlParameter[]
            {
                new MySqlParameter("@rolecode", role),
            };
            string sql = @"SELECT sm.* FROM  sysrole sr , sysmenu sm 
                                    WHERE sr.RoleCode = @rolecode
                                    AND sr.MenuCode  = sm.Code AND sm.Status = 'Y'
                                    ORDER BY Seq ";

            var list = _context.Database.SqlQuery<SysMenu>(sql, parameter);
            return list;
        }
    }
}
