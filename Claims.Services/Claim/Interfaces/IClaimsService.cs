using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Core.Result;

namespace Claims.Services.Claim.Interfaces
{
    public interface IClaimsService
    {
        /// <summary>
        /// Gets all claims.
        /// </summary>
        /// <returns>A successful result containing all claims, or an empty list if none exist.</returns>
        Task<Result<IEnumerable<ClaimResponse>>> GetClaimsAsync();

        /// <summary>
        /// Creates a new claim.
        /// </summary>
        /// <param name="request">The parameters for the creation of the claim.</param>
        /// <returns>
        /// A successful result containing the newly created claim;
        /// a <see cref="ResultType.NotFound"/> result if no cover with the given <see cref="CreateClaimRequest.CoverId"/> exists;
        /// or an <see cref="ResultType.Invalid"/> result if the created date falls outside the cover period.
        /// </returns>
        Task<Result<ClaimResponse>> CreateClaimAsync(CreateClaimRequest request);

        /// <summary>
        /// Deletes a claim by id.
        /// </summary>
        /// <param name="id">The id of the claim to delete.</param>
        /// <returns>
        /// A successful result if the claim was deleted;
        /// or a <see cref="ResultType.NotFound"/> result if no claim with the given id exists.
        /// </returns>
        Task<Result<bool>> DeleteClaimAsync(string id);

        /// <summary>
        /// Gets a claim by id.
        /// </summary>
        /// <param name="id">The id of the claim to retrieve.</param>
        /// <returns>
        /// A successful result containing the claim;
        /// or a <see cref="ResultType.NotFound"/> result if no claim with the given id exists.
        /// </returns>
        Task<Result<ClaimResponse>> GetClaimAsync(string id);
    }
}
