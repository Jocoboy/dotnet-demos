using ABPDemo.EntityFrameworkCore;
using System;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.Timing;

namespace ABPDemo.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(ABPDemoEntityFrameworkCoreModule),
    typeof(ABPDemoApplicationContractsModule)
    )]
public class ABPDemoDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ConfigureClockOptions();
    }

    private void ConfigureClockOptions()
    {
        // PGSQL issues with 5.1.2 (and earlier, due to Npgsql 6+) #11437
        // Cannot write DateTime with Kind=Local to PostgreSQL type 'timestamp with time zone', only UTC is supported.
        Configure<AbpClockOptions>(options =>
        {
            options.Kind = DateTimeKind.Utc;
        });
    }
}
