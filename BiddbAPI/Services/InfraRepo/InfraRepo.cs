using System.Net;
using BiddbAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BiddbAPI.Services;

public class InfraRepo : IInfraRepo
{
    private readonly ILogger<InfraRepo> _logger;
    private readonly string? INFRA_CONN;

    public InfraRepo(ILogger<InfraRepo> logger, IConfiguration configuration)
    {
        _logger = logger;
        INFRA_CONN = configuration["INFRA_CONN"];
    }
    
    public async Task<HttpStatusCode> UpdateMaxBid(string auctionId, int maxBid)
    {
        try
        {
            _logger.LogInformation($"InfraRepo: Updating max bid for auction: {auctionId}, to: {maxBid}");
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.PatchAsync($"{INFRA_CONN}/auctions/{auctionId}/?maxBid={maxBid}", null);
            return response.StatusCode;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw new Exception(e.Message);
        }

    }

    public async Task<Bid> GetMaxBid(string auctionId)
    {
        try
        {
            HttpClient httpClient = new();
            var response = await httpClient.GetFromJsonAsync<Bid>($"{INFRA_CONN}/bids/{auctionId}");
            return response!;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw new Exception(e.Message);
        }
    }
}