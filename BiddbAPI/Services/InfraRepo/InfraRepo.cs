using System.Net;
using BidDbAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BidDbAPI.Services;

public class InfraRepo : IInfraRepo
{
    private readonly string INFRA_CONN = Environment.GetEnvironmentVariable("INFRA_CONN") ?? "http://localhost:5000/api/infra/";
    public async Task<HttpStatusCode> UpdateMaxBid(string auctionId, int maxBid)
    {
        try
        {
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.PatchAsync($"{INFRA_CONN}/auctions/{auctionId}/?maxBid={maxBid}", null);
            return response.StatusCode;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }

    }

    public async Task<Bid> GetMaxBid(string auctionId)
    {
        try
        {
            HttpClient httpClient = new();
            var response = await httpClient.GetAsync($"{INFRA_CONN}/bids/{auctionId}").Result.Content.ReadFromJsonAsync<Bid>();
            return response!;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }


    }
}