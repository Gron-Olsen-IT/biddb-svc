using System.Text;
using System.Text.Json;
using BiddbAPI.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BiddbAPI.Models;
public class RabbitMQBot : IRabbitMQBot
{
    public string? _hostName;
    private readonly ILogger<RabbitMQBot> _logger;
    private ConnectionFactory _factory;

    public RabbitMQBot(ILogger<RabbitMQBot> logger, IConfiguration configuration)
    {
        _hostName = configuration["RABBITMQ_HOSTNAME"];
        _logger = logger;
        _factory = new ConnectionFactory { HostName = _hostName, Port = 5672, UserName = "guest", Password = "guest" };
        _logger.LogInformation($"RabbitMQBot created with hostname: {_hostName}");
    }


    public async Task<Bid?> CheckForMessage(string messageQueue)
    {
        _logger.LogInformation($"Checking for message in queue: {messageQueue}");
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();

        var tcs = new TaskCompletionSource<Bid>();

        channel.QueueDeclare(queue: messageQueue,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            try
            {
                var bidDTO = JsonSerializer.Deserialize<BidDTO>(message);
                if (bidDTO != null)
                {
                    tcs.TrySetResult(new Bid(bidDTO));
                }
                else
                {
                    tcs.TrySetResult(null!);
                }

            }
            catch (Exception e)
            {
                _logger.LogError("Something is wrong with the message", e);
                tcs.TrySetException(e);
            }

        };
        channel.BasicConsume(queue: messageQueue,
                             autoAck: true,
                             consumer: consumer);

        try
        {
            return await tcs.Task;
        }
        catch(Exception e)
        {
            _logger.LogError("Something is wrong with the message", e);
            return null;
        }
    }
}