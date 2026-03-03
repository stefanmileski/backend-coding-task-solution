using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Services.Claim.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController(IClaimsService _claimsService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetClaimResponse>>> GetAsync()
        {
            return Ok(await _claimsService.GetClaimsAsync());
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateClaimRequest request)
        {
            string claimId = await _claimsService.CreateClaimAsync(request);

            if (!string.IsNullOrEmpty(claimId))
            {
                return Ok(claimId);
            }

            return BadRequest("Failed to create claim.");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            bool isDeleted = await _claimsService.DeleteClaimAsync(id);

            if (isDeleted)
            {
                return NoContent();
            }

            return NotFound();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetClaimResponse?>> GetAsync(string id)
        {
            GetClaimResponse? claim = await _claimsService.GetClaimAsync(id);

            if (claim != null)
            {
                return Ok(claim);
            }

            return NotFound();
        }
    }
}
