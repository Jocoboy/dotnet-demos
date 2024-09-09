using Volo.Abp.Modularity;

namespace ABPDemo;

[DependsOn(
    typeof(ABPDemoDomainModule),
    typeof(ABPDemoTestBaseModule)
)]
public class ABPDemoDomainTestModule : AbpModule
{

}
