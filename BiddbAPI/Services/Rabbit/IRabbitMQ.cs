
namespace BiddbAPI.Models;
public interface IRabbitMQBot
{
    Task<Bid?> CheckForMessage(string messageQueue);
}
