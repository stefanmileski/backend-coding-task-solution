using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Infrastructure.Interfaces;
using Claims.Core.Result;
using Claims.Services.Claim.Interfaces;
using Claims.Services.Extensions;

namespace Claims.Services.Claim
{
    public class ClaimsService(IClaimsContext _claimsContext) : IClaimsService
    {
        async Task<Result<ClaimResponse>> IClaimsService.CreateClaimAsync(CreateClaimRequest request)
        {
            Domain.Cover? cover = await _claimsContext.GetCoverAsync(request.CoverId);

            if (cover is null)
            {
                return Result<ClaimResponse>.NotFound(ResultCodes.COVER_NOT_FOUND);
            }

            if (request.Created.Date < cover.StartDate.Date || request.Created.Date > cover.EndDate.Date)
            {
                return Result<ClaimResponse>.Invalid(ResultCodes.CLAIM_CREATED_NOT_WITHIN_COVER_PERIOD); ;
            }

            Domain.Claim claim = request.ToDomain();

            Domain.Claim createdClaim = await _claimsContext.AddClaimAsync(claim);

            ClaimResponse response = createdClaim.ToResponse();

            return Result<ClaimResponse>.Ok(response);
        }

        async Task<Result<bool>> IClaimsService.DeleteClaimAsync(string id)
        {
            bool isDeleted = await _claimsContext.DeleteClaimAsync(id);
            if (!isDeleted)
            {
                return Result<bool>.NotFound(ResultCodes.CLAIM_NOT_FOUND);
            }
            return Result<bool>.Ok(true);
        }

        async Task<Result<ClaimResponse>> IClaimsService.GetClaimAsync(string id)
        {
            Domain.Claim? claim = await _claimsContext.GetClaimAsync(id);
            if (claim is null)
            {
                return Result<ClaimResponse>.NotFound(ResultCodes.CLAIM_NOT_FOUND);
            }
            return Result<ClaimResponse>.Ok(claim.ToResponse());
        }

        async Task<Result<IEnumerable<ClaimResponse>>> IClaimsService.GetClaimsAsync()
        {
            IEnumerable<Domain.Claim> claims = await _claimsContext.GetClaimsAsync();
            return Result<IEnumerable<ClaimResponse>>.Ok(claims.Select(claim => claim.ToResponse()));
        }
    }
}