using Claims.Contracts.Requests;
using Claims.Contracts.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Claims.Services.Extensions
{
    public static class ClaimMappings
    {
        public static Domain.Claim ToDomain(this CreateClaimRequest request) => new(
            id: Guid.NewGuid().ToString(),
            coverId: request.CoverId,
            created: request.Created,
            name: request.Name,
            type: request.Type,
            damageCost: request.DamageCost);

        public static ClaimResponse ToResponse(this Domain.Claim claim) => new(
            id: claim.Id,
            coverId: claim.CoverId,
            created: claim.Created,
            name: claim.Name,
            type: claim.Type,
            damageCost: claim.DamageCost);
    }
}
