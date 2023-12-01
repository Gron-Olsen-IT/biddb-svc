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


    public Bid CheckForMessage(string messageQueue)
    {
        var factory = new ConnectionFactory { HostName = _hostName };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();


        channel.QueueDeclare(queue: messageQueue,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        Bid? bid = null!;
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            try
            {
                bid = JsonSerializer.Deserialize<Bid>(message);
                _logger.LogInformation($"Received message from {messageQueue}: {message}");

            }
            catch (Exception e)
            {
                _logger.LogError("Something is wrong with the message", e);
            }
            
        };
        

        channel.BasicConsume(queue: messageQueue,
                             autoAck: true,
                             consumer: consumer);
        
        return bid;

    }
}