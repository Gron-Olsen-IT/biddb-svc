
using NLog;
using NLog.Web;
using BiddbAPI.Services;
using BiddbAPI.Models;
using System.Threading.RateLimiting;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");
Console.WriteLine("Starting delay");
Task.Delay(20000).Wait();
try
{
    IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        Console.WriteLine("Injecting BidDbWorker");
        services.AddHostedService<BidDbWorker>();
        Console.WriteLine("Injecting BidDbService");
        services.AddScoped<IBidDbService, BidDbService>();
        Console.WriteLine("Injecting InfraRepo");
        services.AddScoped<IInfraRepo, InfraRepo>();
        Console.WriteLine("Injecting BidDbRepoMongo");
        services.AddScoped<IBidDbRepo, BidDbRepoMongo>();
        Console.WriteLine("Injecting RabbitMQBot");
        services.AddScoped<IRabbitMQBot, RabbitMQBot>();
        
    })
    .Build();

    host.Run(); // Run the application
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}