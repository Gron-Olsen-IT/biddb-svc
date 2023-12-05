
namespace BidDbAPI.Models;
public interface IRabbitMQBot
{
    Bid? CheckForMessage(string messageQueue);
}
