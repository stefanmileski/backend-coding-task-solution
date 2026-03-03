using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Domain
{
    public class Claim(string id, string coverId, DateTime created, string name, ClaimType type, decimal damageCost)
    {
        /// <summary>
        /// Id of the claim.
        /// </summary>
        [BsonId]
        public string Id { get; set; } = id;

        /// <summary>
        /// Id of the cover associated with the claim.
        /// </summary>
        [BsonElement("coverId")]
        public string CoverId { get; set; } = coverId;

        /// <summary>
        /// The date and time when the claim was created.
        /// </summary>
        [BsonElement("created")]
        public DateTime Created { get; set; } = created;

        /// <summary>
        /// The name of the claim.
        /// </summary>
        [BsonElement("name")]
        public string Name { get; set; } = name;

        /// <summary>
        /// The type of the claim.
        /// </summary>
        [BsonElement("claimType")]
        public ClaimType Type { get; set; } = type;

        /// <summary>
        /// The estimated cost of the damage associated with the claim.
        /// </summary>
        [BsonElement("damageCost")]
        public decimal DamageCost { get; set; } = damageCost;
    }
}
