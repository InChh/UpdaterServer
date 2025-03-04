﻿using Volo.Abp.Identity;

namespace UpdaterServer;

public static class UpdaterServerConsts
{
    public const string DbTablePrefix = "App";
    public const string? DbSchema = "UpdaterServer";
    public const string AdminUserNameDefaultValue = IdentityDataSeedContributor.AdminUserNameDefaultValue;
    public const string AdminPasswordDefaultValue = IdentityDataSeedContributor.AdminPasswordDefaultValue;
}
