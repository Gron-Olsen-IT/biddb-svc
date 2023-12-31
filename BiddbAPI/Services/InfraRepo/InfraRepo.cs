using System.Net;
using System.Text.Json;
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

/*
    public async Task<BidDTO> GetMaxBid(string auctionId)
    {
        _logger.LogInformation($"InfraRepo: Getting max bid for auction: {auctionId}");
        try
        {
            HttpClient httpClient = new();
            var response = await httpClient.GetFromJsonAsync<BidDTO>($"{INFRA_CONN}/bids/max/{auctionId}");
            return response!;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw new Exception(e.Message);
        }
    }
    */

    public async Task<BidDTO?> GetMaxBid(string auctionId)
{
    _logger.LogInformation($"InfraRepo: Getting max bid for auction: {auctionId}");
    try
    {
        HttpClient httpClient = new();
        HttpResponseMessage response = await httpClient.GetAsync($"{INFRA_CONN}/bids/max/{auctionId}");
        
        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(responseContent))
            {
                return JsonSerializer.Deserialize<BidDTO?>(responseContent);
            }
            else
            {
                // Handle empty or null response
                return null;
            }
        }
        else
        {
            // Handle non-success status code
            _logger.LogError($"Failed to get max bid: {response.StatusCode}");
            return null;
        }
    }
    catch (Exception e)
    {
        _logger.LogError(e.Message);
        throw;
    }
}

}