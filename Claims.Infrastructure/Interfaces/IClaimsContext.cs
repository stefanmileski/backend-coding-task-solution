using Claims.Domain;

namespace Claims.Infrastructure.Interfaces
{
    public interface IClaimsContext
    {
        Task<IEnumerable<Claim>> GetClaimsAsync();
        Task<Claim?> GetClaimAsync(string id);
        Task<Claim> AddClaimAsync(Claim claim);
        Task<bool> DeleteClaimAsync(string id);
        Task<IEnumerable<Cover>> GetCoversAsync();
        Task<Cover?> GetCoverAsync(string id);
        Task<Cover> AddCoverAsync(Cover cover);
        Task<bool> DeleteCoverAsync(string id);
    }

}