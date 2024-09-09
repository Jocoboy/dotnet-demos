using System.Threading.Tasks;
using Volo.Abp.Localization;
using Volo.Abp.UI.Navigation;

namespace ABPDemo.Web.Menus;

public class ABPDemoMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<DefaultResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                ABPDemoMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fas fa-home",
                order: 0
            )
        );

        return Task.CompletedTask;
    }
}
