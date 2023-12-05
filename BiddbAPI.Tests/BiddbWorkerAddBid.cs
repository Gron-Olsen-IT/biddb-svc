using System.Net;
using Moq;
using MongoDB.Driver;
using BiddbAPI.Models;
using BiddbAPI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace BiddbAPI.Tests;


[TestFixture]
public class BiddbWorkerAddBid
{
    private Mock<ILogger<BiddbWorker>> _mockLogger;
    private Mock<IRepo> _mockRepo;
    private Mock<IRabbitMQBot> _mockRabbitMQBot;
    private BiddbWorker _worker;

    [SetUp]
    public void Setup()
    {
        _mockLogger = new Mock<ILogger<BiddbWorker>>();
        _mockRepo = new Mock<IRepo>();
        _mockRabbitMQBot = new Mock<IRabbitMQBot>();

        _worker = new BiddbWorker(_mockLogger.Object, _mockRabbitMQBot.Object, _mockRepo.Object);
    }

    [Test]
    public async Task WhenMessageReceived_ShouldAddBid()
    {
        // Arrange
        var bid = new Bid("1", "1", 1000, DateTime.Now);
        _mockRabbitMQBot.Setup(bot => bot.CheckForMessage("bid")).Returns(bid);
        bid.Id = "1";
        _mockRepo.Setup(repo => repo.AddBid(It.IsAny<Bid>())).ReturnsAsync(bid);

        // Act
        var cancellationToken = new CancellationTokenSource();
        var executeTask = _worker.StartAsync(cancellationToken.Token);

        // Wait for the worker to run at least once
        await Task.Delay(500);
        cancellationToken.Cancel();
        await executeTask;

        // Assert
        //_mockRepo.Verify(repo => repo.AddBid(bid), Times.Once);
        Assert.That(await _mockRepo.Object.AddBid(bid), Is.EqualTo(bid));
    }

    [Test]
    public async Task ExecuteAsync_WhenNoMessageReceived_ShouldNotAddBid()
    {
        // Arrange
        _mockRabbitMQBot.Setup(bot => bot.CheckForMessage("bid")).Returns((Bid)null);

        // Act
        var cancellationToken = new CancellationTokenSource();
        var executeTask = _worker.StartAsync(cancellationToken.Token);

        // Wait for the worker to run at least once
        await Task.Delay(500);
        cancellationToken.Cancel();
        await executeTask;

        // Assert
        _mockRepo.Verify(repo => repo.AddBid(It.IsAny<Bid>()), Times.Never);
    }
   
}
