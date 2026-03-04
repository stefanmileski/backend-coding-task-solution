using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Controllers.Base;
using Claims.Services.Claim.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController(IClaimsService _claimsService) : ApiControllerBase
    {
        [HttpGet("list")]
        [ProducesResponseType(typeof(IEnumerable<ClaimResponse>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClaimResponse>>> GetClaimsAsync()
        {
            return OkOrError(await _claimsService.GetClaimsAsync());
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateClaimAsync(CreateClaimRequest request)
        {
            return OkOrError(await _claimsService.CreateClaimAsync(request));
        }

        [HttpDelete("{id}/delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteClaimAsync(string id)
        {
            return OkOrError(await _claimsService.DeleteClaimAsync(id));
        }

        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(ClaimResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClaimResponse>> GetClaimAsync(string id)
        {
            return OkOrError(await _claimsService.GetClaimAsync(id));
        }
    }
}
