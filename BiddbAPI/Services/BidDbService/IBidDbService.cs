
using BiddbAPI.Models;

namespace BiddbAPI.Services;
public interface IBidDbService
{
    Task<Bid> Post(Bid bid);
}