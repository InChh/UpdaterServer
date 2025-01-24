using Volo.Abp.Settings;

namespace UpdaterServer.Settings;

public class UpdaterServerSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(UpdaterServerSettings.MySetting1));
    }
}
