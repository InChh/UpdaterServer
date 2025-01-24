using Microsoft.Extensions.Localization;
using UpdaterServer.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace UpdaterServer;

[Dependency(ReplaceServices = true)]
public class UpdaterServerBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<UpdaterServerResource> _localizer;

    public UpdaterServerBrandingProvider(IStringLocalizer<UpdaterServerResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
