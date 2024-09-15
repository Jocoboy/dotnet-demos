using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Repository.IRepository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Repository.IRepository
{
    public interface ISysUserRepository : IBaseRepository<SysUser>
    {
    }
}
