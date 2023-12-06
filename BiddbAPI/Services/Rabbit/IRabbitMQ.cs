
namespace BiddbAPI.Models;
public interface IRabbitMQBot
{
    Task<BidDTO?> CheckForMessage(string messageQueue);
}
