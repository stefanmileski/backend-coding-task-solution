using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Services.Claim.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController(IClaimsService _claimsService) : ControllerBase
    {
        [HttpGet("list")]
        [ProducesResponseType(typeof(IEnumerable<ClaimResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClaimResponse>>> GetClaimsAsync()
        {
            return Ok(await _claimsService.GetClaimsAsync());
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateClaimAsync(CreateClaimRequest request)
        {
            ClaimResponse? claim = await _claimsService.CreateClaimAsync(request);

            if (claim is null)
            {
                return BadRequest("Failed to create claim.");
            }

            return Ok(claim);
        }

        [HttpDelete("{id}/delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteClaimAsync(string id)
        {
            bool isDeleted = await _claimsService.DeleteClaimAsync(id);

            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(ClaimResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClaimResponse>> GetClaimAsync(string id)
        {
            ClaimResponse? claim = await _claimsService.GetClaimAsync(id);

            if (claim == null)
            {
                return NotFound();
            }

            return Ok(claim);
        }
    }
}
