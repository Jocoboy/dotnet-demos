using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Repository.IRepository;
using AspNetCoreDemo.Repository.Repository.Base;
using AspNetCoreDemo.Repository.Uow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Repository.Repository
{
    public class SysUserRepository : BaseRepository<SysUser>, ISysUserRepository
    {
        public SysUserRepository(IUnitofWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
