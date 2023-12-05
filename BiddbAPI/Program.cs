
using NLog;
using NLog.Web;
using BidDbAPI.Services;
using BidDbAPI.Models;
using System.Threading.RateLimiting;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");
Task.Delay(20000).Wait();
try
{
    IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<BidDbWorker>();
        services.AddScoped<IBidDbRepo, BidDbRepoMongo>();
        services.AddSingleton<IRabbitMQBot>();
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