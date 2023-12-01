
namespace BiddbAPI.Services;
using BiddbAPI.Models;

public class BiddbWorker : BackgroundService
{
    private readonly ILogger<BiddbWorker> _logger;
    private readonly RabbitMQBot _rabbitMQBot;

    public BiddbWorker(ILogger<BiddbWorker> logger, ILogger<RabbitMQBot> loggerRabbitMQBot, IRepo repo)
    {
        _logger = logger;
        string hostName = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME") ?? "localhost";
        _rabbitMQBot = new RabbitMQBot(hostName, loggerRabbitMQBot, repo);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BiddbWorker running at: {time}", DateTimeOffset.Now);
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("BiddbWorker task doing background work.");
            _rabbitMQBot.CheckMessage("bid");
            await Task.Delay(5000, stoppingToken);
        }
    }
}
