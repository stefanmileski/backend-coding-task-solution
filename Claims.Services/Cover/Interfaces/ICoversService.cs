namespace Claims.Services.Cover.Interfaces
{
    public interface ICoversService
    {
        Task<IEnumerable<Domain.Cover>> GetCoversAsync();

        Task CreateCoverAsync(Domain.Cover cover);

        Task DeleteCoverAsync(Guid uid);

        Task<Domain.Cover?> GetCoverAsync(Guid uid);

        decimal ComputePremium(DateTime startDate, DateTime endDate, Domain.CoverType coverType);
    }
}
