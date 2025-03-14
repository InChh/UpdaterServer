using Aspire.Hosting.Azure;
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);
// https://github.com/dotnet/aspire/issues/4505
// 所有容器名称需要小写

// var dbUsername = builder.AddParameter("DbUsername", true);
// var dbPassword = builder.AddParameter("DbPassword", true);
// IResourceBuilder<AzurePostgresFlexibleServerDatabaseResource> postgres;
// if (builder.Environment.IsDevelopment())
// {
//     postgres = builder.AddAzurePostgresFlexibleServer("updater-db")
//         .RunAsContainer()
//         .WithPasswordAuthentication(dbUsername, dbPassword)
//         .AddDatabase("updater");
// }
// else
// {
//     postgres = builder
//         .AddAzurePostgresFlexibleServer("updater-db")
//         .WithPasswordAuthentication(dbUsername, dbPassword)
//         .AddDatabase("updater");
// }
var postgres = builder
    .AddPostgres("updater-db")
    .WithDataVolume()
    .AddDatabase("updater");


var redis = builder.AddRedis("redis");

var accessKeyId = builder.AddParameter("OssAccessKeyId", true);
var accessKeySecret = builder.AddParameter("OssAccessKeySecret", true);
var uploadRamRoleArn = builder.AddParameter("OssUploadRamRoleArn", true);
var downloadRamRoleArn = builder.AddParameter("OssDownloadRamRoleArn", true);
var ossStsEndpoint = builder.AddParameter("OssStsEndpoint", true);
var environment = builder.AddParameter("Environment");

// DbMigrator
var migrator = builder
    .AddProject<Projects.UpdaterServer_DbMigrator>("db-migrator")
    .WithEnvironment("DOTNET_ENVIRONMENT", environment)
    .WaitFor(postgres)
    .WaitFor(redis)
    .WithReference(postgres, "Default")
    .WithReference(redis, "Redis")
    .WithReplicas(1);

const string httpApiHostLaunchProfile = "UpdaterServer.HttpApi.Host";
builder
    .AddProject<Projects.UpdaterServer_HttpApi_Host>("api", httpApiHostLaunchProfile)
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", environment)
    .WithEnvironment("OSS_ACCESS_KEY_ID", accessKeyId)
    .WithEnvironment("OSS_ACCESS_KEY_SECRET", accessKeySecret)
    .WithEnvironment("OSS_UPLOAD_RAM_ROLE_ARN", uploadRamRoleArn)
    .WithEnvironment("OSS_DOWNLOAD_RAM_ROLE_ARN", downloadRamRoleArn)
    .WithEnvironment("OSS_STS_ENDPOINT", ossStsEndpoint)
    .WithExternalHttpEndpoints()
    .WaitForCompletion(migrator)
    .WithReference(postgres, "Default")
    .WithReference(redis, "Redis");

builder.Build().Run();