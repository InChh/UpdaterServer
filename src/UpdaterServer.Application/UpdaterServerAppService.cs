using UpdaterServer.Localization;
using Volo.Abp.Application.Services;

namespace UpdaterServer;

/* Inherit your application services from this class.
 */
public abstract class UpdaterServerAppService : ApplicationService
{
    protected UpdaterServerAppService()
    {
        LocalizationResource = typeof(UpdaterServerResource);
    }
}
