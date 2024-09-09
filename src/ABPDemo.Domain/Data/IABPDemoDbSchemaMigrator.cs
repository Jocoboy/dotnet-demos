using System.Threading.Tasks;

namespace ABPDemo.Data;

public interface IABPDemoDbSchemaMigrator
{
    Task MigrateAsync();
}
