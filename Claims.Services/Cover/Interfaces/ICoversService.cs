using Claims.Contracts.Requests;
using Claims.Contracts.Responses;

namespace Claims.Services.Cover.Interfaces
{
    public interface ICoversService
    {
        /// <summary>
        /// Gets all covers.
        /// </summary>
        /// <returns>All covers, or an empty list.</returns>
        Task<IEnumerable<GetCoverResponse>> GetCoversAsync();

        /// <summary>
        /// Creates a new cover.
        /// </summary>
        /// <param name="request">The parameters for the creation of the cover.</param>
        /// <returns>The id of the newly created cover, or null.</returns>
        Task<string> CreateCoverAsync(CreateCoverRequest cover);

        /// <summary>
        /// Deletes a cover by id.
        /// </summary>
        /// <param name="id">The id of the cover to delete.</param>
        /// <returns>True if the deletion is successful, otherwise false.</returns>
        Task<bool> DeleteCoverAsync(string id);

        /// <summary>
        /// Gets a cover by id.
        /// </summary>
        /// <param name="id">The id of the cover to get.</param>
        /// <returns>The cover with the given id, or null.</returns>
        Task<GetCoverResponse?> GetCoverAsync(string id);

        /// <summary>
        /// Calculates the insurance premium for a specified coverage type over a given date range.
        /// </summary>
        /// <param name="startDate">The start date of the coverage period.</param>
        /// <param name="endDate">The end date of the coverage period.</param>
        /// <param name="coverType">The type of coverage for which the premium is to be calculated.</param>
        /// <returns>The calculated premium amount for the specified coverage period and type.</returns>
        decimal ComputePremium(DateTime startDate, DateTime endDate, Domain.CoverType coverType);
    }
}
