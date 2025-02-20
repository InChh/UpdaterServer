using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);
var postgres = builder
    .AddPostgres("UpdaterDb")
    .WithDataVolume("UpdaterDb")
    .AddDatabase("Updater");

var redis = builder.AddRedis("redis");

var isDevelopment = builder.Environment.IsDevelopment();
var envs = Environment.GetEnvironmentVariables();

// DbMigrator
var migrator = builder
    .AddProject<Projects.UpdaterServer_DbMigrator>("DbMigrator")
    .WithEnvironment("DOTNET_ENVIRONMENT", isDevelopment ? "Development" : "Production")
    .WaitFor(postgres)
    .WaitFor(redis)
    .WithReference(postgres, "Default")
    .WithReference(redis, "Redis")
    .WithReplicas(1);

const string httpApiHostLaunchProfile = "UpdaterServer.HttpApi.Host";
builder
    .AddProject<Projects.UpdaterServer_HttpApi_Host>("httpApiHost", httpApiHostLaunchProfile)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", isDevelopment ? "Development" : "Production")
    .WithEnvironment("OSS_ACCESS_KEY_ID", envs["OSS_ACCESS_KEY_ID"]?.ToString())
    .WithEnvironment("OSS_ACCESS_KEY_SECRET", envs["OSS_ACCESS_KEY_SECRET"]?.ToString())
    .WithEnvironment("OSS_UPLOAD_RAM_ROLE_ARN", envs["OSS_UPLOAD_RAM_ROLE_ARN"]?.ToString())
    .WithEnvironment("OSS_DOWNLOAD_RAM_ROLE_ARN", envs["OSS_DOWNLOAD_RAM_ROLE_ARN"]?.ToString())
    .WithEnvironment("TZ", envs["TZ"]?.ToString())
    .WithExternalHttpEndpoints()
    .WaitForCompletion(migrator)
    .WithReference(postgres, "Default")
    .WithReference(redis, "Redis");

builder.Build().Run();