
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

    public async Task<Bid> Post(Bid bid)
    {
        try
        {
            _logger.LogInformation($"DbService: Posting bid: {bid}");
            Bid previousMaxBid = new(await _infraRepo.GetMaxBid(bid.AuctionId));
            _logger.LogInformation($"DbService: Previous max bid: {previousMaxBid.Offer}");
            if(previousMaxBid.Offer >= bid.Offer)
            {
                throw new Exception("Offer is lower than current max bid");
            }
            Bid postedBid = await _bidDbRepo.AddBid(bid) ?? throw new Exception("Bid was not posted");
            await _infraRepo.UpdateMaxBid(bid.AuctionId, postedBid.Offer);
            return postedBid;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw new Exception(e.Message);
        }
    }
}