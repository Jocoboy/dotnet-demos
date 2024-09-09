using Volo.Abp.AspNetCore.Mvc;

namespace ABPDemo.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class ABPDemoController : AbpControllerBase
{
    protected ABPDemoController()
    {
    }
}
