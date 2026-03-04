using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Domain;
using Claims.Infrastructure.Interfaces;
using Claims.Infrastructure.Result;
using Claims.Services.Cover.Interfaces;
using Claims.Services.Extensions;
namespace Claims.Services.Cover
{
    public class CoversService(IClaimsContext _claimsContext) : ICoversService
    {
        async Task<Result<CoverResponse>> ICoversService.CreateCoverAsync(CreateCoverRequest request)
        {
            Domain.Cover cover = request.ToDomain(ComputePremium(request.StartDate, request.EndDate, request.Type));

            Domain.Cover createdCover = await _claimsContext.AddCoverAsync(cover);

            CoverResponse response = createdCover.ToResponse();

            return Result<CoverResponse>.Ok(response);
        }

        async Task<Result<bool>> ICoversService.DeleteCoverAsync(string id)
        {
            bool isDeleted = await _claimsContext.DeleteCoverAsync(id);

            if (!isDeleted)
            {
                return Result<bool>.NotFound(ResultCodes.COVER_NOT_FOUND);
            }

            return Result<bool>.Ok(true);
        }

        async Task<Result<CoverResponse>> ICoversService.GetCoverAsync(string id)
        {
            Domain.Cover? cover = await _claimsContext.GetCoverAsync(id);

            if (cover is null)
            {
                return Result<CoverResponse>.NotFound(ResultCodes.COVER_NOT_FOUND);
            }

            return Result<CoverResponse>.Ok(cover.ToResponse());
        }

        async Task<Result<IEnumerable<CoverResponse>>> ICoversService.GetCoversAsync()
        {
            IEnumerable<Domain.Cover> covers = await _claimsContext.GetCoversAsync();

            return Result<IEnumerable<CoverResponse>>.Ok(covers.Select(cover => cover.ToResponse()));
        }

        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            const decimal baseDayRate = 1250m;

            decimal multiplier = coverType switch
            {
                CoverType.Yacht => 1.1m,
                CoverType.PassengerShip => 1.2m,
                CoverType.Tanker => 1.5m,
                _ => 1.3m
            };

            bool isYacht = coverType == CoverType.Yacht;

            // Days 30–179
            decimal firstDiscount = isYacht ? 0.05m : 0.02m;

            // Days 180+ (additional 3%/1% on top)
            decimal secondDiscount = isYacht ? 0.08m : 0.03m;

            decimal premiumPerDay = baseDayRate * multiplier;

            // 1 is added to include the end date in the calculation
            int insuranceDays = (endDate.Date - startDate.Date).Days + 1;

            decimal totalPremium = 0m;

            for (int day = 0; day < insuranceDays; day++)
            {
                decimal dayRate = day switch
                {
                    < 30 => premiumPerDay,
                    < 180 => premiumPerDay * (1 - firstDiscount),
                    _ => premiumPerDay * (1 - secondDiscount),
                };

                totalPremium += dayRate;
            }

            return totalPremium;
        }
    }
}