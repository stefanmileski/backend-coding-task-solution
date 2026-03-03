using Claims.Domain;
using Claims.Services.Claim.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController(IClaimsService _claimsService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Claim>>> GetAsync()
        {
            return Ok(await _claimsService.GetClaimsAsync());
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(Claim claim)
        {
            await _claimsService.CreateClaimAsync(claim);
            
            return Ok(claim);
        }

        [HttpDelete("{uid}")]
        public async Task DeleteAsync(Guid uid)
        {
            await _claimsService.DeleteClaimAsync(uid);
        }

        [HttpGet("{uid}")]
        public async Task<ActionResult<Claim?>> GetAsync(Guid uid)
        {
            return await _claimsService.GetClaimAsync(uid);
        }
    }
}
