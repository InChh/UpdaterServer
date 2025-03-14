using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UpdaterServer.Application;
using UpdaterServer.ApplicationVersion;
using UpdaterServer.File;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;

namespace UpdaterServer.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ConnectionStringName("Default")]
public class UpdaterServerDbContext(DbContextOptions<UpdaterServerDbContext> options) :
    AbpDbContext<UpdaterServerDbContext>(options),
    IIdentityDbContext, IDataProtectionKeyContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */
    public DbSet<Application.Application> Applications { get; set; }
    public DbSet<ApplicationVersion.ApplicationVersion> ApplicationVersions { get; set; }
    public DbSet<FileMetadata> FileMetadatas { get; set; }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    #region Entities from the modules

    /* Notice: We only implemented IIdentityProDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityProDbContext .
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureBlobStoring();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(UpdaterServerConsts.DbTablePrefix + "YourEntities", UpdaterServerConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});

        #region DataProtectionKey

        builder.Entity<DataProtectionKey>(b =>
        {
            b.ToTable(UpdaterServerConsts.DbTablePrefix + "DataProtectionKeys", UpdaterServerConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasKey(x => x.Id);
            b.Property(x => x.FriendlyName);
            b.Property(x => x.Xml);
        });

        #endregion

        #region Application

        builder.Entity<Application.Application>(b =>
        {
            b.ToTable(UpdaterServerConsts.DbTablePrefix + "Applications", UpdaterServerConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Name).IsRequired().HasMaxLength(ApplicationConsts.MaxNameLength);
            b.Property(x => x.Description).HasMaxLength(ApplicationConsts.MaxDescriptionLength);
        });

        #endregion

        #region ApplicationVersion

        builder.Entity<ApplicationVersion.ApplicationVersion>(b =>
        {
            b.ToTable(UpdaterServerConsts.DbTablePrefix + "ApplicationVersions", UpdaterServerConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.VersionNumber).IsRequired().HasMaxLength(ApplicationVersionConsts.MaxVersionNumberLength);
            b.Property(x => x.Description).HasMaxLength(ApplicationVersionConsts.MaxDescriptionLength);
            b.HasOne<Application.Application>().WithMany().HasForeignKey(x => x.ApplicationId).IsRequired();
            b.Property(x => x.IsActive).IsRequired();
            b.HasMany(x => x.Files).WithOne().HasForeignKey(x => x.VersionId).IsRequired();
        });

        #endregion

        #region FileMetadata

        builder.Entity<FileMetadata>(b =>
        {
            b.ToTable(UpdaterServerConsts.DbTablePrefix + "FileMetadatas", UpdaterServerConsts.DbSchema);
            b.ConfigureByConvention();
            b.Property(x => x.Path).IsRequired().HasMaxLength(FileMetadataConsts.MaxPathLength);
            b.Property(x => x.Hash).IsRequired().HasMaxLength(FileMetadataConsts.MaxHashLength);
            b.Property(x => x.Size).IsRequired();
            b.Property(x => x.Url).IsRequired().HasMaxLength(FileMetadataConsts.MaxUrlLength);
        });

        #endregion

        #region VersionFile

        builder.Entity<VersionFile>(b =>
        {
            b.ToTable(UpdaterServerConsts.DbTablePrefix + "VersionFiles", UpdaterServerConsts.DbSchema);
            b.ConfigureByConvention();
            b.HasKey(x => new { x.VersionId, x.FileMetadataId });
            b.HasOne<ApplicationVersion.ApplicationVersion>().WithMany().HasForeignKey(x => x.VersionId).IsRequired();
            b.HasOne<FileMetadata>().WithMany().HasForeignKey(x => x.FileMetadataId).IsRequired();
            b.HasIndex(x => new { x.VersionId, x.FileMetadataId });
        });

        #endregion
    }
}