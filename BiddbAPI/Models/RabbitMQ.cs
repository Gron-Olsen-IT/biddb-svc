using System.Text;
using System.Text.Json;
using BiddbAPI.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BiddbAPI.Models;
public class RabbitMQBot
{
    public string _hostName { get; }
    private readonly ILogger<RabbitMQBot> _logger;

    public RabbitMQBot(ILogger<RabbitMQBot> logger)
    {
        _hostName = Environment.GetEnvironmentVariable("RABBITMQ_HOSTNAME") ?? "localhost";
        _logger = logger;
    }


    public Bid? CheckForMessage(string messageQueue)
    {
        var factory = new ConnectionFactory { HostName = _hostName, Port = 5672, UserName = "guest", Password = "guest" };
        using var connection = factory.CreateConnection();
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
            Console.WriteLine($" [x] Received: '{message}'");
            
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