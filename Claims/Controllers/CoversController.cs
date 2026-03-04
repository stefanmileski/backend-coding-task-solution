using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Controllers.Base;
using Claims.Domain;
using Claims.Core.Result;
using Claims.Services.Cover.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    /// <summary>
    /// Manages insurance covers and premium computation.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class CoversController(ICoversService _coversService) : ApiControllerBase
    {
        /// <summary>
        /// Computes the insurance premium for a given cover type and date range.
        /// </summary>
        /// <param name="startDate">The start date of the coverage period.</param>
        /// <param name="endDate">The end date of the coverage period.</param>
        /// <param name="coverType">The type of cover.</param>
        /// <returns>The computed premium amount.</returns>
        /// <remarks>
        /// The premium is calculated progressively based on the length of the period:
        /// the first 30 days use the base rate, the following 150 days apply a discount,
        /// and any remaining days apply a further discount.
        /// </remarks>
        [HttpPost("compute")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        public async Task<ActionResult> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            return OkOrError(_coversService.ComputePremium(startDate, endDate, coverType));
        }

        /// <summary>
        /// Returns all covers.
        /// </summary>
        /// <returns>A list of all covers, or an empty list if none exist.</returns>
        [HttpGet("list")]
        [ProducesResponseType(typeof(Result<IEnumerable<CoverResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CoverResponse>>> GetCoversAsync()
        {
            return OkOrError(await _coversService.GetCoversAsync());
        }
        
        /// <summary>
        /// Returns a cover by id.
        /// </summary>
        /// <param name="id">The id of the cover to retrieve.</param>
        /// <returns>The cover with the specified id.</returns>
        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(Result<CoverResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CoverResponse?>> GetCoverAsync(string id)
        {
            return OkOrError(await _coversService.GetCoverAsync(id));
        }

        /// <summary>
        /// Creates a new cover.
        /// </summary>
        /// <param name="request">The cover creation parameters.</param>
        /// <returns>The newly created cover.</returns>
        /// <remarks>
        /// StartDate cannot be in the past.
        /// The total insurance period cannot exceed one year.
        /// </remarks>
        [HttpPost("create")]
        [ProducesResponseType(typeof(Result<CoverResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateCoverAsync(CreateCoverRequest request)
        {
            return OkOrError(await _coversService.CreateCoverAsync(request));
        }

        /// <summary>
        /// Deletes a cover by id.
        /// </summary>
        /// <param name="id">The id of the cover to delete.</param>
        /// <returns>True if the cover was deleted successfully.</returns>
        [HttpDelete("{id}/delete")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCoverAsync(string id)
        {
            return OkOrError(await _coversService.DeleteCoverAsync(id));
        }
    }
}