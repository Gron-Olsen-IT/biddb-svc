using System.Text;
using System.Text.Json;
using BiddbAPI.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BiddbAPI.Models;
public class RabbitMQBot : IRabbitMQBot
{
    public string _hostName { get; }
    private readonly ILogger<RabbitMQBot> _logger;
    private ConnectionFactory _factory;

    public RabbitMQBot(ILogger<RabbitMQBot> logger)
    {
        _hostName = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME") ?? "localhost";
        _logger = logger;
        _factory = new ConnectionFactory { HostName = _hostName, Port = 5672, UserName = "guest", Password = "guest" };
        _logger.LogInformation($"RabbitMQBot created with hostname: {_hostName}");
    }


    public Bid? CheckForMessage(string messageQueue)
    {
        using var connection = _factory.CreateConnection();
        using var channel = connection.CreateModel();

        string message = "";

        channel.QueueDeclare(queue: messageQueue,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            message = Encoding.UTF8.GetString(body);
            
        };
        channel.BasicConsume(queue: messageQueue,
                             autoAck: true,
                             consumer: consumer);
        if (message == "")
        {
            return null;
        }
        try
        {
            return JsonSerializer.Deserialize<Bid>(message);
        }
        catch (Exception e)
        {
            _logger.LogError("Something is wrong with the message", e);
            return null;
        }

    }
}