using Claims.Infrastructure;
using Claims.Services.Claim.Interfaces;

namespace Claims.Services.Claim
{
    public class ClaimsService(ClaimsContext _claimsContext) : IClaimsService
    {
        async Task IClaimsService.CreateClaimAsync(Domain.Claim claim)
        {
            claim.Id = Guid.NewGuid().ToString();
            await _claimsContext.AddClaimAsync(claim);
        }

        async Task IClaimsService.DeleteClaimAsync(string id)
        {
            await _claimsContext.DeleteClaimAsync(id);
        }

        async Task<Domain.Claim?> IClaimsService.GetClaimAsync(string id)
        {
            return await _claimsContext.GetClaimAsync(id);
        }

        async Task<IEnumerable<Domain.Claim>> IClaimsService.GetClaimsAsync()
        {
            return await _claimsContext.GetClaimsAsync();
        }
    }
}
