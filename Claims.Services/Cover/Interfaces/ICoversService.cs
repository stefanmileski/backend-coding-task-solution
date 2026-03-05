using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Core.Result;

namespace Claims.Services.Cover.Interfaces
{
    public interface ICoversService
    {
        /// <summary>
        /// Gets all covers.
        /// </summary>
        /// <returns>A successful result containing all covers, or an empty list if none exist.</returns>
        Task<Result<IEnumerable<CoverResponse>>> GetCoversAsync();

        /// <summary>
        /// Creates a new cover.
        /// </summary>
        /// <param name="cover">The parameters for the creation of the cover.</param>
        /// <returns>
        /// A successful result containing the newly created cover;
        /// or an <see cref="ResultType.Invalid"/> result if the end date is before the start date.
        /// </returns>
        Task<Result<CoverResponse>> CreateCoverAsync(CreateCoverRequest cover);

        /// <summary>
        /// Deletes a cover by id.
        /// </summary>
        /// <param name="id">The id of the cover to delete.</param>
        /// <returns>
        /// A successful result if the cover was deleted;
        /// or a <see cref="ResultType.NotFound"/> result if no cover with the given id exists.
        /// </returns>
        Task<Result<bool>> DeleteCoverAsync(string id);

        /// <summary>
        /// Gets a cover by id.
        /// </summary>
        /// <param name="id">The id of the cover to retrieve.</param>
        /// <returns>
        /// A successful result containing the cover;
        /// or a <see cref="ResultType.NotFound"/> result if no cover with the given id exists.
        /// </returns>
        Task<Result<CoverResponse>> GetCoverAsync(string id);

        /// <summary>
        /// Calculates the insurance premium for a specified coverage type over a given date range.
        /// </summary>
        /// <param name="startDate">The start date of the coverage period.</param>
        /// <param name="endDate">The end date of the coverage period.</param>
        /// <param name="coverType">The type of coverage for which the premium is to be calculated.</param>
        /// <returns>
        /// A successful result containing the calculated premium;
        /// or an <see cref="ResultType.Invalid"/> result if the end date is before the start date.
        /// </returns>
        Result<decimal> ComputePremium(DateTime startDate, DateTime endDate, Domain.CoverType coverType);
    }
}
