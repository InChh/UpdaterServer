using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UpdaterServer.Data;
using Serilog;
using Volo.Abp;
using Volo.Abp.Data;

namespace UpdaterServer.DbMigrator;

public class DbMigratorHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _hostEnvironment;

    public DbMigratorHostedService(IHostApplicationLifetime hostApplicationLifetime, IConfiguration configuration,
        IHostEnvironment hostEnvironment)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _configuration = configuration;
        _hostEnvironment = hostEnvironment;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var application = await AbpApplicationFactory.CreateAsync<UpdaterServerDbMigratorModule>(options =>
        {
            var builder = new ConfigurationBuilder();
            builder
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{_hostEnvironment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            options.Services.ReplaceConfiguration(_configuration);
            options.UseAutofac();
            options.Services.AddLogging(c => c.AddSerilog());
            options.AddDataMigrationEnvironment();
            options.Services.AddSingleton(_hostEnvironment);
        });
        await application.InitializeAsync();

        await application
            .ServiceProvider
            .GetRequiredService<UpdaterServerDbMigrationService>()
            .MigrateAsync();

        await application.ShutdownAsync();

        _hostApplicationLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}