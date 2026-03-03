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
        [ProducesResponseType(typeof(IEnumerable<GetClaimResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<GetClaimResponse>>> GetClaimsAsync()
        {
            return Ok(await _claimsService.GetClaimsAsync());
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateClaimAsync(CreateClaimRequest request)
        {
            string? claimId = await _claimsService.CreateClaimAsync(request);

            if (string.IsNullOrEmpty(claimId))
            {
                return BadRequest("Failed to create claim.");
            }

            return Created();
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
        [ProducesResponseType(typeof(GetClaimResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GetClaimResponse>> GetClaimAsync(string id)
        {
            GetClaimResponse? claim = await _claimsService.GetClaimAsync(id);

            if (claim == null)
            {
                return NotFound();
            }

            return Ok(claim);
        }
    }
}
