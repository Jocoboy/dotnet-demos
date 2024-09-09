using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace ABPDemo.ExceptionHandle
{
    public class ExpectedExceptionCollection : Collection<ExpectedExceptionDescription>, IExpectedExceptionCollection, IScopedDependency
    {
    }
}
