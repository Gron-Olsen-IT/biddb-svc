
namespace BiddbAPI.Services;
using BiddbAPI.Models;

public class BidDbWorker : BackgroundService
{
    private readonly ILogger<BidDbWorker> _logger;
    private readonly IBidDbRepo _bidDbRepo;
    private readonly IRabbitMQBot _rabbitMQBot;

    public BidDbWorker(ILogger<BidDbWorker> logger, IRabbitMQBot rabbitMQBot, IBidDbRepo bidDbRepo)
    {
        _logger = logger;
        _bidDbRepo = bidDbRepo;
        _rabbitMQBot = rabbitMQBot;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BidDbWorker running at: {time}", DateTimeOffset.Now);
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("BidDbWorker task doing background work.");
            Bid? message = await _rabbitMQBot.CheckForMessage("bid");
            if (message != null)
            {
                _logger.LogInformation($"Received message from bid: {message}");
                await _bidDbRepo.AddBid(message);
            }
            await Task.Delay(10000, stoppingToken);
        }
    }
}
