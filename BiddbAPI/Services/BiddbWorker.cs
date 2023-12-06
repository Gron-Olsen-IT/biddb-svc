
namespace BiddbAPI.Services;
using BiddbAPI.Models;

public class BidDbWorker : BackgroundService
{
    private readonly ILogger<BidDbWorker> _logger;
    private readonly IBidDbService _bidDbService;
    private readonly IRabbitMQBot _rabbitMQBot;

    public BidDbWorker(ILogger<BidDbWorker> logger, IRabbitMQBot rabbitMQBot, IBidDbService bidDbService)
    {
        _logger = logger;
        _bidDbService = bidDbService;
        _rabbitMQBot = rabbitMQBot;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("BidDbWorker running at: {time}", DateTimeOffset.Now);
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("BidDbWorker task doing background work.");
            BidDTO? message = await _rabbitMQBot.CheckForMessage("bid");
            if (message != null)
            {
                try
                {
                    _logger.LogInformation($"Received message from bid: {message}");
                    await _bidDbService.Post(message);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
            }
            await Task.Delay(500, stoppingToken);
        }
    }
}
