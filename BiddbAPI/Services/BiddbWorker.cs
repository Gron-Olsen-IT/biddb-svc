
namespace BiddbAPI.Services;
using BiddbAPI.Models;

public class BiddbWorker : BackgroundService
{
    private readonly ILogger<BiddbWorker> _logger;
    private readonly IRepo _repo;
    private readonly RabbitMQBot _rabbitMQBot;

    public BiddbWorker(ILogger<BiddbWorker> logger, ILogger<RabbitMQBot> loggerRabbitMQBot, IRepo repo, RabbitMQBot rabbitMQBot)
    {
        _repo = repo;
        _logger = logger;
        _rabbitMQBot = rabbitMQBot;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BiddbWorker running at: {time}", DateTimeOffset.Now);
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("BiddbWorker task doing background work.");
            Bid message = _rabbitMQBot.CheckForMessage("bid");
            if (message != null)
            {
                _logger.LogInformation($"Received message from bid: {message}");
                await _repo.AddBid(message);
            }
            await Task.Delay(5000, stoppingToken);
        }
    }
}
