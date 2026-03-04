using Claims.Domain;

namespace Claims.Contracts.Requests
{
    public class CreateCoverRequest(DateTime startDate, DateTime endDate, CoverType type)
    {
        /// <summary>
        /// Start date of the cover
        /// </summary>
        public DateTime StartDate { get; set; } = startDate;

        /// <summary>
        /// End date of the cover
        /// </summary>
        public DateTime EndDate { get; set; } = endDate;

        /// <summary>
        /// Type of the cover
        /// </summary>
        public CoverType Type { get; set; } = type;
    }
}
