using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Domain
{
    public class Claim(string id, string coverId, DateTime created, string name, ClaimType type, decimal damageCost)
    {
        [BsonId]
        public string Id { get; set; } = id;

        [BsonElement("coverId")]
        public string CoverId { get; set; } = coverId;

        [BsonElement("created")]
        public DateTime Created { get; set; } = created;

        [BsonElement("name")]
        public string Name { get; set; } = name;

        [BsonElement("claimType")]
        public ClaimType Type { get; set; } = type;

        [BsonElement("damageCost")]
        public decimal DamageCost { get; set; } = damageCost;
    }
}
