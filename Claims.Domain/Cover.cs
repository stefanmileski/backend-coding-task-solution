using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Domain
{
    public class Cover(string id, DateTime startDate, DateTime endDate, CoverType type, decimal premium)
    {
        [BsonId]
        public string Id { get; set; } = id;

        [BsonElement("startDate")]
        public DateTime StartDate { get; set; } = startDate;

        [BsonElement("endDate")]
        public DateTime EndDate { get; set; } = endDate;

        [BsonElement("claimType")]
        public CoverType Type { get; set; } = type;

        [BsonElement("premium")]
        public decimal Premium { get; set; } = premium;
    }
}