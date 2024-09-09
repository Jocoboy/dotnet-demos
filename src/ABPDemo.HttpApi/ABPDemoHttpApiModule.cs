using Volo.Abp.Modularity;

namespace ABPDemo;

[DependsOn(
    typeof(ABPDemoApplicationContractsModule)
)]
public class ABPDemoHttpApiModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {

    }

}
