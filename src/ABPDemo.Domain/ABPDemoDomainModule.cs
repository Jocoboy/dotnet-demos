using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.SettingManagement;

namespace ABPDemo;

[DependsOn(
    typeof(ABPDemoDomainSharedModule),
    typeof(AbpIdentityDomainModule),
    typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(AbpSettingManagementDomainModule)
)]
public class ABPDemoDomainModule : AbpModule
{

}
