
using BiddbAPI.Models;

namespace BiddbAPI.Services;

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
        _logger.LogInformation($"DbService: Posting bid: {bidDTO}");
        var previousMaxBid = await _infraRepo.GetMaxBid(bidDTO.AuctionId);

        // Handle the scenario where there are no existing bids
        if (previousMaxBid == null)
        {
            _logger.LogInformation("DbService: No existing bids for this auction.");
        }
        else
        {
            _logger.LogInformation($"DbService: Previous max bid: {previousMaxBid.Offer}");
            // Check if the new bid is lower than or equal to the current max bid
            if (previousMaxBid.Offer >= bidDTO.Offer)
            {
                throw new Exception("Offer is lower than current max bid");
            }
        }

        Bid postedBid = await _bidDbRepo.AddBid(new Bid(bidDTO)) ?? throw new Exception("Bid was not posted");
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