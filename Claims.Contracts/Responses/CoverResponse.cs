using Claims.Domain;

namespace Claims.Contracts.Responses
{
    public class CoverResponse
    {
        /// <summary>
        /// Cover identifier
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// Start date of the cover
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of the cover
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Type of the cover
        /// </summary>
        public CoverType Type { get; set; }

        /// <summary>
        /// Premium for the cover
        /// </summary>
        public decimal Premium { get; set; }
    }
}
