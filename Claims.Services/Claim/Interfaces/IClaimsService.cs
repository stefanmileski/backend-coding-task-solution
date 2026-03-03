namespace Claims.Services.Claim.Interfaces
{
    public interface IClaimsService
    {
        Task<IEnumerable<Domain.Claim>> GetClaimsAsync();

        Task CreateClaimAsync(Domain.Claim claim);

        Task DeleteClaimAsync(string id);

        Task<Domain.Claim?> GetClaimAsync(string id);
    }
}
