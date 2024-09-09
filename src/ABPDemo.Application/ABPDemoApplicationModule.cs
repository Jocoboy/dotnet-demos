using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;

namespace ABPDemo;

[DependsOn(
    typeof(ABPDemoDomainModule),
    typeof(ABPDemoApplicationContractsModule)
    )]
public class ABPDemoApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<ABPDemoApplicationModule>();
        });
    }
}
