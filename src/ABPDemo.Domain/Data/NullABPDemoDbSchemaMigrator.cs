using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace ABPDemo.Data;

/* This is used if database provider does't define
 * IABPDemoDbSchemaMigrator implementation.
 */
public class NullABPDemoDbSchemaMigrator : IABPDemoDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
