using Volo.Abp.Modularity;

namespace ABPDemo;

public abstract class ABPDemoApplicationTestBase<TStartupModule> : ABPDemoTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
