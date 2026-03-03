using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Domain
{
    public class Cover(string id, DateTime startDate, DateTime endDate, CoverType type, decimal premium)
    {
        /// <summary>
        /// The id of the cover.
        /// </summary>
        [BsonId]
        public string Id { get; set; } = id;

        /// <summary>
        /// The start date of the cover.
        /// </summary>
        [BsonElement("startDate")]
        public DateTime StartDate { get; set; } = startDate;

        /// <summary>
        /// The end date of the cover.
        /// </summary>
        [BsonElement("endDate")]
        public DateTime EndDate { get; set; } = endDate;

        /// <summary>
        /// The type of the cover.
        /// </summary>
        [BsonElement("claimType")]
        public CoverType Type { get; set; } = type;

        /// <summary>
        /// The premium of the cover.
        /// </summary>
        [BsonElement("premium")]
        public decimal Premium { get; set; } = premium;
    }
}