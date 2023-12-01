using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
public class Bid
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string BuyerId { get; set; }
    public string AuctionId { get; set; }
    public int Offer { get; set; }
    public DateTime Time { get; set; }

    public Bid(string buyerId, string auctionId, int offer, DateTime time)
    {
        BuyerId = buyerId;
        AuctionId = auctionId;
        Offer = offer;
        Time = time;
    }
    

}