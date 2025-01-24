using UpdaterServer.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace UpdaterServer.Permissions;

public class UpdaterServerPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(UpdaterServerPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(UpdaterServerPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<UpdaterServerResource>(name);
    }
}
