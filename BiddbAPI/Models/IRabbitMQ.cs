public interface IRabbitMQBot
{
    Bid? CheckForMessage(string messageQueue);
}
