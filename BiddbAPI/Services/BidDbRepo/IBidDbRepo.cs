namespace BiddbAPI.Services;
using BiddbAPI.Models;
public interface IBidDbRepo
{
    Task<IEnumerable<Bid>> GetBids();
    Task<Bid> GetBid(string id);
    Task<Bid> AddBid(Bid bid);
    Task<Bid> UpdateBid(Bid bid);
    Task<Bid> DeleteBid(string id);
}