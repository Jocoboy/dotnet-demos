using AspNetCoreDemo.Model.EFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Repository.Uow
{
    public interface IUnitofWork
    {
        /// <summary>
        /// 保证Db唯一
        /// </summary>
        /// <returns></returns>
        BaseContext GetDbContext();

        /// <summary>
        /// 设置还原点
        /// </summary>
        void BeginTran();

        /// <summary>
        /// 提交事务
        /// </summary>
        void CommitTran();

        /// <summary>
        /// 回滚事务
        /// </summary>
        void RollbackTran();
    }
}
