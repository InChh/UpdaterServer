﻿using System;
using System.Threading.Tasks;
using AbpSolution1.HttpApi.Client.ConsoleTestApp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;

namespace UpdaterServer.HttpApi.Client.ConsoleTestApp;

class Program
{
    static async Task Main(string[] args)
    {
        using var application = await AbpApplicationFactory.CreateAsync<UpdaterServerConsoleApiClientModule>(options =>
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", false);
            builder.AddJsonFile("appsettings.secrets.json", true);
            options.Services.ReplaceConfiguration(builder.Build());
            options.UseAutofac();
        });
        await application.InitializeAsync();

        var demo = application.ServiceProvider.GetRequiredService<ClientDemoService>();
        await demo.RunAsync();

        Console.WriteLine("Press ENTER to stop application...");
        Console.ReadLine();

        await application.ShutdownAsync();
    }
}
