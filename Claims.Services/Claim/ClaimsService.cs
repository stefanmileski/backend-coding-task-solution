using Claims.Infrastructure;
using Claims.Services.Claim.Interfaces;

namespace Claims.Services.Claim
{
    public class ClaimsService(ClaimsContext _claimsContext) : IClaimsService
    {
        async Task IClaimsService.CreateClaimAsync(Domain.Claim claim)
        {
            claim.Uid = Guid.NewGuid();
            await _claimsContext.AddClaimAsync(claim);
        }

        async Task IClaimsService.DeleteClaimAsync(Guid uid)
        {
            await _claimsContext.DeleteClaimAsync(uid);
        }

        async Task<Domain.Claim?> IClaimsService.GetClaimAsync(Guid uid)
        {
            return await _claimsContext.GetClaimAsync(uid);
        }

        async Task<IEnumerable<Domain.Claim>> IClaimsService.GetClaimsAsync()
        {
            return await _claimsContext.GetClaimsAsync();
        }
    }
}
