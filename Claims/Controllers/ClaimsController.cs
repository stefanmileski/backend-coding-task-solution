using Claims.Auditing;
using Claims.Domain.Claim;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController(ClaimsContext claimsContext, AuditContext auditContext) : ControllerBase
    {
        private readonly ClaimsContext _claimsContext = claimsContext;
        private readonly Auditer _auditer = new(auditContext);

        [HttpGet]
        public async Task<IEnumerable<Claim>> GetAsync()
        {
            return await _claimsContext.GetClaimsAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(Claim claim)
        {
            claim.Id = Guid.NewGuid().ToString();
            await _claimsContext.AddItemAsync(claim);
            _auditer.AuditClaim(claim.Id, "POST");
            return Ok(claim);
        }

        [HttpDelete("{id}")]
        public async Task DeleteAsync(string id)
        {
            _auditer.AuditClaim(id, "DELETE");
            await _claimsContext.DeleteItemAsync(id);
        }

        [HttpGet("{id}")]
        public async Task<Claim> GetAsync(string id)
        {
            return await _claimsContext.GetClaimAsync(id);
        }
    }
}
