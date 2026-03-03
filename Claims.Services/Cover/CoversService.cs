using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Domain;
using Claims.Infrastructure;
using Claims.Services.Cover.Interfaces;

namespace Claims.Services.Cover
{
    public class CoversService(ClaimsContext _claimsContext) : ICoversService
    {
        async Task<string> ICoversService.CreateCoverAsync(CreateCoverRequest request)
        {
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
            var multiplier = 1.3m;
            if (coverType == CoverType.Yacht)
            {
                multiplier = 1.1m;
            }

            if (coverType == CoverType.PassengerShip)
            {
                multiplier = 1.2m;
            }

            if (coverType == CoverType.Tanker)
            {
                multiplier = 1.5m;
            }

            var premiumPerDay = 1250 * multiplier;
            var insuranceLength = (endDate.Date - startDate.Date).TotalDays;
            var totalPremium = 0m;

            for (var i = 0; i < insuranceLength; i++)
            {
                if (i < 30) totalPremium += premiumPerDay;
                if (i < 180 && coverType == CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.05m;
                else if (i < 180) totalPremium += premiumPerDay - premiumPerDay * 0.02m;
                if (i < 365 && coverType != CoverType.Yacht) totalPremium += premiumPerDay - premiumPerDay * 0.03m;
                else if (i < 365) totalPremium += premiumPerDay - premiumPerDay * 0.08m;
            }

            return totalPremium;
        }
    }
}