using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);
var postgres = builder
    .AddPostgres("UpdaterDb")
    .WithVolume("/UpdaterDb")
    .AddDatabase("Updater");

var redis = builder.AddRedis("redis");

// DbMigrator
var migrator = builder
        .AddProject<Projects.UpdaterServer_DbMigrator>("DbMigrator")
        .WaitFor(postgres)
        .WaitFor(redis)
        .WithReference(postgres, "Default")
        .WithReference(redis, "Redis")
        .WithReplicas(1);

const string httpApiHostLaunchProfile = "UpdaterServer.HttpApi.Host";
builder
    .AddProject<Projects.UpdaterServer_HttpApi_Host>("httpApiHost", httpApiHostLaunchProfile)
    .WithExternalHttpEndpoints()
    .WaitFor(migrator)
    .WithReference(postgres, "Default")
    .WithReference(redis, "Redis");

builder.Build().Run();