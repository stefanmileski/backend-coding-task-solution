using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Controllers.Base;
using Claims.Core.Result;
using Claims.Services.Claim.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    /// <summary>
    /// Manages insurance claims.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController(IClaimsService _claimsService) : ApiControllerBase
    {
        /// <summary>
        /// Returns all claims.
        /// </summary>
        /// <returns>A list of all claims, or an empty list if none exist.</returns>
        [HttpGet("list")]
        [ProducesResponseType(typeof(Result<IEnumerable<ClaimResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClaimResponse>>> GetClaimsAsync()
        {
            return OkOrError(await _claimsService.GetClaimsAsync());
        }

        /// <summary>
        /// Creates a new claim.
        /// </summary>
        /// <param name="request">The claim creation parameters.</param>
        /// <returns>The newly created claim.</returns>
        /// <remarks>
        /// DamageCost cannot exceed 100,000.
        /// The Created date must fall within the period of the related cover.
        /// </remarks>
        [HttpPost("create")]
        [ProducesResponseType(typeof(Result<ClaimResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateClaimAsync(CreateClaimRequest request)
        {
            return OkOrError(await _claimsService.CreateClaimAsync(request));
        }

        /// <summary>
        /// Deletes a claim by id.
        /// </summary>
        /// <param name="id">The id of the claim to delete.</param>
        /// <returns>True if the claim was deleted successfully.</returns>
        [HttpDelete("{id}/delete")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteClaimAsync(string id)
        {
            return OkOrError(await _claimsService.DeleteClaimAsync(id));
        }

        /// <summary>
        /// Returns a claim by id.
        /// </summary>
        /// <param name="id">The id of the claim to retrieve.</param>
        /// <returns>The claim with the specified id.</returns>
        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(Result<ClaimResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClaimResponse>> GetClaimAsync(string id)
        {
            return OkOrError(await _claimsService.GetClaimAsync(id));
        }
    }
}
