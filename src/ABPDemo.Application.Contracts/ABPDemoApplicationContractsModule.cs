using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;

namespace ABPDemo;

[DependsOn(
    typeof(ABPDemoDomainSharedModule),
    typeof(AbpObjectExtendingModule)
)]
public class ABPDemoApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        ABPDemoDtoExtensions.Configure();
    }
}
