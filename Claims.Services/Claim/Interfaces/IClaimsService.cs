namespace Claims.Services.Claim.Interfaces
{
    public interface IClaimsService
    {
        Task<IEnumerable<Domain.Claim>> GetClaimsAsync();

        Task CreateClaimAsync(Domain.Claim claim);

        Task DeleteClaimAsync(Guid uid);

        Task<Domain.Claim?> GetClaimAsync(Guid uid);
    }
}
