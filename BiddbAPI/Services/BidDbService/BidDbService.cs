
using BidDbAPI.Models;

namespace BidDbAPI.Services;

public class BidDbService : IBidDbService
{

    private readonly IBidDbRepo _bidDbRepo;
    private readonly ILogger<BidDbService> _logger;
    private readonly IInfraRepo _infraRepo;

    public BidDbService(IBidDbRepo bidDbRepo, ILogger<BidDbService> logger, IInfraRepo infraRepo)
    {
        _bidDbRepo = bidDbRepo;
        _logger = logger;
        _infraRepo = infraRepo;
    }

    public async Task<Bid> Post(BidDTO bidDTO)
    {
        try
        {
            Bid previousMaxBid = await _infraRepo.GetMaxBid(bidDTO.AuctionId);
            if(previousMaxBid.Offer >= bidDTO.Offer)
            {
                throw new Exception("Offer is lower than current max bid");
            }
            Bid postedBid = await _bidDbRepo.AddBid(new(bidDTO));
            await _infraRepo.UpdateMaxBid(bidDTO.AuctionId, postedBid.Offer);
            return postedBid;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw new Exception(e.Message);
        }
    }
}