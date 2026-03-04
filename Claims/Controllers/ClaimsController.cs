using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Controllers.Base;
using Claims.Core.Result;
using Claims.Services.Claim.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController(IClaimsService _claimsService) : ApiControllerBase
    {
        [HttpGet("list")]
        [ProducesResponseType(typeof(Result<IEnumerable<ClaimResponse>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClaimResponse>>> GetClaimsAsync()
        {
            return OkOrError(await _claimsService.GetClaimsAsync());
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(Result<ClaimResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateClaimAsync(CreateClaimRequest request)
        {
            return OkOrError(await _claimsService.CreateClaimAsync(request));
        }

        [HttpDelete("{id}/delete")]
        [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteClaimAsync(string id)
        {
            return OkOrError(await _claimsService.DeleteClaimAsync(id));
        }

        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(Result<ClaimResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClaimResponse>> GetClaimAsync(string id)
        {
            return OkOrError(await _claimsService.GetClaimAsync(id));
        }
    }
}
