using System.Net;
using BidDbAPI.Models;

namespace BidDbAPI.Services;


public interface IInfraRepo
{

    Task<HttpStatusCode> UpdateMaxBid(string auctionId, int maxBid);
    Task<Bid> GetMaxBid(string auctionId);
}