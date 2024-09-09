using Volo.Abp.Modularity;

namespace ABPDemo;

/* Inherit from this class for your domain layer tests. */
public abstract class ABPDemoDomainTestBase<TStartupModule> : ABPDemoTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
