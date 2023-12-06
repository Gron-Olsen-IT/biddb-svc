
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
            Bid previousMaxBid = await _infraRepo.GetMaxBid(bidDTO.AuctionId);
            _logger.LogInformation($"DbService: Previous max bid: {previousMaxBid.Offer}");
            if(previousMaxBid.Offer >= bidDTO.Offer)
            {
                throw new Exception("Offer is lower than current max bid");
            }
            Bid postedBid = await _bidDbRepo.AddBid(new(bidDTO)) ?? throw new Exception("Bid was not posted");
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