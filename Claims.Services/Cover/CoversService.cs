using Claims.Domain;
using Claims.Infrastructure;
using Claims.Services.Cover.Interfaces;

namespace Claims.Services.Cover
{
    public class CoversService(ClaimsContext _claimsContext) : ICoversService
    {
        async Task ICoversService.CreateCoverAsync(Domain.Cover cover)
        {
            cover.Id = Guid.NewGuid().ToString();
            cover.Premium = ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
            await _claimsContext.AddCoverAsync(cover);
        }

        async Task ICoversService.DeleteCoverAsync(string id)
        {
            await _claimsContext.DeleteCoverAsync(id);
        }

        async Task<Domain.Cover?> ICoversService.GetCoverAsync(string id)
        {
            return await _claimsContext.GetCoverAsync(id);
        }

        async Task<IEnumerable<Domain.Cover>> ICoversService.GetCoversAsync()
        {
            return await _claimsContext.GetCoversAsync();
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
            var insuranceLength = (endDate - startDate).TotalDays;
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