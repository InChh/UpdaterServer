using UpdaterServer.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace UpdaterServer.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class UpdaterServerController : AbpControllerBase
{
    protected UpdaterServerController()
    {
        LocalizationResource = typeof(UpdaterServerResource);
    }
}
