using System.Net;
using BiddbAPI.Models;

namespace BiddbAPI.Services;


public interface IInfraRepo
{

    Task<HttpStatusCode> UpdateMaxBid(string auctionId, int maxBid);
    Task<Bid> GetMaxBid(string auctionId);
}