
namespace BiddbAPI.Models;
public interface IRabbitMQBot
{
    Bid? CheckForMessage(string messageQueue);
}
