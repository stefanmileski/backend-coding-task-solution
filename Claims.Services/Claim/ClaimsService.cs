using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Infrastructure;
using Claims.Services.Claim.Interfaces;

namespace Claims.Services.Claim
{
    public class ClaimsService(ClaimsContext _claimsContext) : IClaimsService
    {
        async Task<string?> IClaimsService.CreateClaimAsync(CreateClaimRequest request)
        {
            if (request.DamageCost > 100000)
            {
                return null;
            }

            Domain.Cover? cover = await _claimsContext.GetCoverAsync(request.CoverId);

            if (cover is null
                || (request.Created < cover.StartDate || request.Created > cover.EndDate))
            {
                return null;
            }

            Domain.Claim claim = new(
                id: Guid.NewGuid().ToString(),
                coverId: request.CoverId,
                created: request.Created,
                name: request.Name,
                type: request.Type,
                damageCost: request.DamageCost);

            return await _claimsContext.AddClaimAsync(claim);
        }

        async Task<bool> IClaimsService.DeleteClaimAsync(string id)
        {
            return await _claimsContext.DeleteClaimAsync(id);
        }

        async Task<GetClaimResponse?> IClaimsService.GetClaimAsync(string id)
        {
            Domain.Claim? claim = await _claimsContext.GetClaimAsync(id);

            if (claim is null)
            {
                return null;
            }

            return new GetClaimResponse(
                id: claim.Id,
                coverId: claim.CoverId,
                created: claim.Created,
                name: claim.Name,
                type: claim.Type,
                damageCost: claim.DamageCost);
        }

        async Task<IEnumerable<GetClaimResponse>> IClaimsService.GetClaimsAsync()
        {
            IEnumerable<Domain.Claim> claims = await _claimsContext.GetClaimsAsync();

            return claims.Select(claim => new GetClaimResponse(
                id: claim.Id,
                coverId: claim.CoverId,
                created: claim.Created,
                name: claim.Name,
                type: claim.Type,
                damageCost: claim.DamageCost));
        }
    }
}
