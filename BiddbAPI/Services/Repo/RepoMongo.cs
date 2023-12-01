namespace BiddbAPI.Services;
using BiddbAPI.Models;
using MongoDB.Driver;


public class RepoMongo : IRepo {
    private readonly IMongoCollection<Bid>? _collection;
    private readonly ILogger<RepoMongo> _logger;

    public RepoMongo(ILogger<RepoMongo> logger) {
        _logger = logger;
        try
        {
            var hostName = System.Net.Dns.GetHostName();
            var ips = System.Net.Dns.GetHostAddresses(hostName);
            var _ipaddr = ips.First().MapToIPv4().ToString();
            _logger.LogInformation($"Biddb-svc responding from {_ipaddr}");
        }
        catch (Exception e)
        {
            _logger.LogError("Something is wrong with the IP-address", e);
        }
        try
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")! ?? "mongodb+srv://Boes24:vxeF7fR49HHPL75h6Lg5492Rx6v8BcdQ@gronolsen.5ub48h1.mongodb.net";
            var mongoDatabase = new MongoClient(connectionString).GetDatabase("bid_db");
            _collection = mongoDatabase.GetCollection<Bid>("bids");

        }
        catch (Exception e)
        {
            _logger.LogError("Something is wrong with the CONNECTION_STRING", e);
        }
    }



    public async Task<IEnumerable<Bid>> GetBids() {
        return await _collection.Find(bid => true).ToListAsync();
    }

    public async Task<Bid> GetBid(string id) {
        return await _collection.Find<Bid>(bid => bid.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Bid> AddBid(Bid bid) {
        await _collection!.InsertOneAsync(bid);
        return bid;
    }

    public async Task<Bid> UpdateBid(Bid bid) {
        await _collection.ReplaceOneAsync(b => b.Id == bid.Id, bid);
        return bid;
    }

    public async Task<Bid> DeleteBid(string id) {
        return await _collection.FindOneAndDeleteAsync(bid => bid.Id == id);
    }
}