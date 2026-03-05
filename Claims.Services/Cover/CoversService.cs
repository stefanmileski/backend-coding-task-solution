using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using Claims.Domain;
using Claims.Infrastructure.Interfaces;
using Claims.Core.Result;
using Claims.Services.Cover.Interfaces;
using Claims.Services.Extensions;
namespace Claims.Services.Cover
{
    public class CoversService(IClaimsContext _claimsContext) : ICoversService
    {
        async Task<Result<CoverResponse>> ICoversService.CreateCoverAsync(CreateCoverRequest request)
        {
            Result<decimal> premiumForCover = ComputePremium(request.StartDate, request.EndDate, request.Type);

            if (!premiumForCover.IsSuccess)
            {
                return Result<CoverResponse>.Invalid(premiumForCover.Message!);
            }

            Domain.Cover cover = request.ToDomain(premiumForCover.Value);

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

        public Result<decimal> ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            if (endDate.Date < startDate.Date)
            {
                return Result<decimal>.Invalid(ResultCodes.END_DATE_BEFORE_START_DATE);
            }

            decimal premiumPerDay = GetDailyPremium(coverType);

            int insuranceDays = (endDate.Date - startDate.Date).Days + 1;

            decimal secondTierDiscount = coverType == CoverType.Yacht
                ? YachtSecondTierDiscount
                : OtherSecondTierDiscount;

            decimal thirdTierDiscount = coverType == CoverType.Yacht
                ? YachtThirdTierDiscount
                : OtherThirdTierDiscount;

            decimal totalPremium = 0m;

            for (int day = 0; day < insuranceDays; day++)
            {
                totalPremium += GetDiscountedDailyRate(day, premiumPerDay, secondTierDiscount, thirdTierDiscount);
            }

            return Result<decimal>.Ok(totalPremium);
        }

        private decimal GetDiscountedDailyRate(
            int dayIndex,
            decimal premiumPerDay,
            decimal secondTierDiscount,
            decimal thirdTierDiscount)
        {
            if (dayIndex < FirstTierDays)
                return premiumPerDay;

            if (dayIndex < SecondTierDays)
                return premiumPerDay * (1 - secondTierDiscount);

            return premiumPerDay * (1 - thirdTierDiscount);
        }

        private decimal GetDailyPremium(CoverType coverType)
        {
            const decimal BaseDayRate = 1250m;

            return coverType switch
            {
                CoverType.Yacht => BaseDayRate * 1.1m,
                CoverType.PassengerShip => BaseDayRate * 1.2m,
                CoverType.Tanker => BaseDayRate * 1.5m,
                _ => BaseDayRate * 1.3m
            };
        }

        private const int FirstTierDays = 30;
        private const int SecondTierDays = 180;

        private const decimal YachtSecondTierDiscount = 0.05m;
        private const decimal OtherSecondTierDiscount = 0.02m;

        private const decimal YachtThirdTierDiscount = 0.08m;
        private const decimal OtherThirdTierDiscount = 0.03m;
    }
}