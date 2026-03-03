using Claims.Infrastructure;
using Claims.Services.Claim.Interfaces;

namespace Claims.Services.Claim
{
    public class ClaimsService(ClaimsContext claimsContext) : IClaimsService
    {
        async Task IClaimsService.CreateClaimAsync(Domain.Claim claim)
        {
            claim.Uid = Guid.NewGuid();
            await claimsContext.AddItemAsync(claim);
        }

        async Task IClaimsService.DeleteClaimAsync(Guid uid)
        {
            await claimsContext.DeleteItemAsync(uid);
        }

        async Task<Domain.Claim?> IClaimsService.GetClaimAsync(Guid uid)
        {
            return await claimsContext.GetClaimAsync(uid);
        }

        async Task<IEnumerable<Domain.Claim>> IClaimsService.GetClaimsAsync()
        {
            return await claimsContext.GetClaimsAsync();
        }
    }
}
