using AspNetCoreDemo.Model.EFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Repository.Uow
{
    public class UnitofWork : IUnitofWork
    {
        private readonly IBaseContext _context;

        public UnitofWork(IBaseContext context)
        {
            _context = context;
        }

        public BaseContext GetDbContext()
        {
            return _context as BaseContext;
        }

        public void BeginTran()
        {
            GetDbContext().Database.BeginTransaction();
        }

        public void CommitTran()
        {
            try
            {
                GetDbContext().Database.CommitTransaction();
            }
            catch (Exception)
            {
                GetDbContext().Database.RollbackTransaction();
                throw;
            }
        }

        public void RollbackTran()
        {
            GetDbContext().Database.RollbackTransaction();
        }
    }
}
