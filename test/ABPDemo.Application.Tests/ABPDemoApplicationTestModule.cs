using Volo.Abp.Modularity;

namespace ABPDemo;

[DependsOn(
    typeof(ABPDemoApplicationModule),
    typeof(ABPDemoDomainTestModule)
)]
public class ABPDemoApplicationTestModule : AbpModule
{

}
