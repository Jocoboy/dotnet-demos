using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace ABPDemo.Web;

[Dependency(ReplaceServices = true)]
public class ABPDemoBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "ABPDemo";
}
