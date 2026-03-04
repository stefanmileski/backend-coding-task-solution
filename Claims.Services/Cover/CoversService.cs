using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Domain;
using Claims.Infrastructure;
using Claims.Services.Cover.Interfaces;

namespace Claims.Services.Cover
{
    public class CoversService(ClaimsContext _claimsContext) : ICoversService
    {
        async Task<string?> ICoversService.CreateCoverAsync(CreateCoverRequest request)
        {
            DateTime startDate = request.StartDate.Date;
            DateTime endDate = request.EndDate.Date;

            if (startDate < DateTime.UtcNow.Date)
            {
                return null;
            }

            if (endDate > startDate.AddYears(1))
            {
                return null;
            }

            if (endDate < startDate)
            {
                return null;
            }

            Domain.Cover cover = new(
                id: Guid.NewGuid().ToString(),
                startDate: request.StartDate,
                endDate: request.EndDate,
                type: request.Type,
                premium: ComputePremium(request.StartDate, request.EndDate, request.Type));

            return await _claimsContext.AddCoverAsync(cover);
        }

        async Task<bool> ICoversService.DeleteCoverAsync(string id)
        {
            return await _claimsContext.DeleteCoverAsync(id);
        }

        async Task<GetCoverResponse?> ICoversService.GetCoverAsync(string id)
        {
            Domain.Cover? cover = await _claimsContext.GetCoverAsync(id);

            if (cover is null)
            {
                return null;
            }

            return new GetCoverResponse
            {
                Id = cover.Id,
                StartDate = cover.StartDate,
                EndDate = cover.EndDate,
                Type = cover.Type,
                Premium = cover.Premium
            };
        }

        async Task<IEnumerable<GetCoverResponse>> ICoversService.GetCoversAsync()
        {
            IEnumerable<Domain.Cover> covers = await _claimsContext.GetCoversAsync();

            return covers.Select(cover => new GetCoverResponse
            {
                Id = cover.Id,
                StartDate = cover.StartDate,
                EndDate = cover.EndDate,
                Type = cover.Type,
                Premium = cover.Premium
            });
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
            int insuranceDays = (endDate.Date - startDate.Date).Days;
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