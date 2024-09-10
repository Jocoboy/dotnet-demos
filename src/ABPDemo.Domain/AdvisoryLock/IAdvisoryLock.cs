using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ABPDemo.AdvisoryLock
{
    /// <summary>
    /// 基于 PGSQL Advisory Lock 范式的锁.
    /// 用一个long类型的数值或两个int类型的数值标识一把锁. long类型标识的锁和int类型标识的锁互相独立.
    /// 锁有两个锁定级别:会话级和事务级. 会话级锁定会持续到会话结束或显式释放时并且不受会话中事务的影响, 事务级锁定不能显式释放会持续锁定到事务结束.
    /// 锁有两种锁定模式:独占和共享.独占锁定和其它的独占锁定或共享锁定都互斥, 共享锁定只和独占锁定互斥, 共享锁定之间不互斥. 锁定模式不受锁定级别影响, 
    /// 既同一把锁的会话级独占锁定和事务级独占锁定会正确的互斥.
    /// 锁是可重入的, 不论哪个级别的锁定都是如此. 
    /// </summary>
    public interface IAdvisoryLock
    {
        /// <summary>
        /// 对long类型的数值标识的锁进行会话级别的锁定
        /// </summary>
        /// <returns></returns>
        Task<ISessionLock> LockAsync(long k, LockMode lockMode, bool waiting, CancellationToken cancellationToken = default);

        /// <summary>
        /// 对由两个int类型的数值标识的锁进行会话级别的锁定
        /// </summary>
        Task<ISessionLock> LockAsync(int m, int n, LockMode lockMode, bool waiting, CancellationToken cancellationToken = default);

        /// <summary>
        /// 对long类型的数值标识的锁进行事务级别的锁定
        /// </summary>
        /// <returns></returns>
        Task<bool> XactLockAsync(long k, LockMode lockMode, bool waiting, CancellationToken cancellationToken = default);

        /// <summary>
        /// 对由两个int类型的数值标识的锁进行事务级别的锁定
        /// </summary>
        Task<bool> XactLockAsync(int m, int n, LockMode lockMode, bool waiting, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 锁定模式
    /// </summary>
    public enum LockMode
    {
        /// <summary>
        /// 独占
        /// </summary>
        Exclusive = 0,

        /// <summary>
        /// 共享
        /// </summary>
        Shared = 1,
    }
}
