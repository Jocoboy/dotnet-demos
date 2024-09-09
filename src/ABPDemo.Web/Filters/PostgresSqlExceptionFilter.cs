using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Runtime.ExceptionServices;
using Volo.Abp.DependencyInjection;
using Volo.Abp;
using ABPDemo.ExceptionHandle;
using System.Linq;

namespace ABPDemo.Web.Filters
{
    /// <summary>
    /// 过滤PGSQL的错误异常
    /// </summary>
    public class PostgresSqlExceptionFilter : IExceptionFilter, ITransientDependency
    {
        private readonly IExpectedExceptionCollection _expectedExceptions;

        public PostgresSqlExceptionFilter(IExpectedExceptionCollection expectedExceptions)
        {
            _expectedExceptions = expectedExceptions;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is DbUpdateException due && due.InnerException is PostgresException pe)
            {
                switch (pe.SqlState)
                {
                    case "23505": ProcessByConstranitName(context, pe, ExpectedExceptionType.UniqueViolation); break;
                    case "23503": ProcessByConstranitName(context, pe, ExpectedExceptionType.ForeignKeyViolation); break;
                };
            }
        }

        private void ProcessByConstranitName(ExceptionContext context, PostgresException pe, ExpectedExceptionType type)
        {
            var ee = _expectedExceptions.FirstOrDefault(e => e.Type == type && e.Key == pe.ConstraintName);

            if (ee == null) return;

            context.Exception = new UserFriendlyException(ee.ErrorMessage, ee.ErrorCode, innerException: context.Exception);
            context.ExceptionDispatchInfo = ExceptionDispatchInfo.Capture(context.Exception);
        }
    }
}
