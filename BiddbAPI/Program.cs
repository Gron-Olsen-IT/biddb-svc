
using NLog;
using NLog.Web;
using BiddbAPI.Services;
using BiddbAPI.Models;
using System.Threading.RateLimiting;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");
Task.Delay(5000).Wait();
try
{
    IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<BiddbWorker>();
        services.AddScoped<IRepo, RepoMongo>();
        services.AddSingleton<RabbitMQBot>();
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