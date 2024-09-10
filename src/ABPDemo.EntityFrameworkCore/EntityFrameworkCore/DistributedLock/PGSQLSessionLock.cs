using ABPDemo.AdvisoryLock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPDemo.EntityFrameworkCore.DistributedLock
{
    public class PGSQLSessionLock : ISessionLock
    {
        private DatabaseFacade database;
        private long? kid;
        private int? mid;
        private int? nid;
        private bool isShare;
        private bool disposedValue;

        public PGSQLSessionLock(DatabaseFacade database, long kid, bool isShare)
        {
            this.database = database;
            this.kid = kid;
            this.isShare = isShare;
        }
        public PGSQLSessionLock(DatabaseFacade database, int mid, int nid, bool isShare)
        {
            this.database = database;
            this.mid = mid;
            this.nid = nid;
            this.isShare = isShare;
        }

        private void ReleaseLock()
        {
            if (isShare)
            {
                if (kid != null)
                {
                    database.ExecuteSqlRaw("select pg_advisory_unlock_shared({0})", new object[] { kid.Value });
                }
                else
                {
                    database.ExecuteSqlRaw("select pg_advisory_unlock_shared({0},{1})", new object[] { mid.Value, nid.Value });
                }
            }
            else
            {
                if (kid != null)
                {
                    database.ExecuteSqlRaw("select pg_advisory_unlock({0})", new object[] { kid.Value });
                }
                else
                {
                    database.ExecuteSqlRaw("select pg_advisory_unlock({0},{1})", new object[] { mid.Value, nid.Value });
                }
            }

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                ReleaseLock();
                database = null;
                disposedValue = true;
            }
        }

        // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        ~PGSQLSessionLock()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
