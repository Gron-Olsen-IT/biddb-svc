
using BidDbAPI.Models;

namespace BidDbAPI.Services;
public interface IBidDbService
{
    Task<Bid> Post(BidDTO bidDTO);
}