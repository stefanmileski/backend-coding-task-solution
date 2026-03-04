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
        /// <returns>All claims, or an empty list.</returns>
        Task<Result<IEnumerable<ClaimResponse>>> GetClaimsAsync();

        /// <summary>
        /// Creates a new claim.
        /// </summary>
        /// <param name="request">The parameters for the creation of the claim.</param>
        /// <returns>The id of the newly created claim, or null if unsuccessful.</returns>
        Task<Result<ClaimResponse>> CreateClaimAsync(CreateClaimRequest request);

        /// <summary>
        /// Deletes a claim by id.
        /// </summary>
        /// <param name="id">The id of the claim to delete.</param>
        /// <returns>True if the deletion is successful, otherwise false.</returns>
        Task<Result<bool>> DeleteClaimAsync(string id);

        /// <summary>
        /// Gets a claim by id.
        /// </summary>
        /// <param name="id">The id of the claim to get.</param>
        /// <returns>The claim with the given id, or null if not found.</returns>
        Task<Result<ClaimResponse>> GetClaimAsync(string id);
    }
}
