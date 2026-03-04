using Claims.Contracts.Requests;
using Claims.Contracts.Responses;

namespace Claims.Services.Extensions
{
    public static class CoverMappings
    {
        public static Domain.Cover ToDomain(this CreateCoverRequest request, decimal premium) => new(
            id: Guid.NewGuid().ToString(),
            startDate: request.StartDate,
            endDate: request.EndDate,
            type: request.Type,
            premium: premium);

        public static CoverResponse ToResponse(this Domain.Cover cover) => new()
        {
            Id = cover.Id,
            StartDate = cover.StartDate,
            EndDate = cover.EndDate,
            Type = cover.Type,
            Premium = cover.Premium
        };
    }
}
