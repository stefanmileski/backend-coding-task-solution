namespace Claims.Services.Cover.Interfaces
{
    public interface ICoversService
    {
        Task<IEnumerable<Domain.Cover>> GetCoversAsync();

        Task CreateCoverAsync(Domain.Cover cover);

        Task DeleteCoverAsync(string id);

        Task<Domain.Cover?> GetCoverAsync(string id);

        decimal ComputePremium(DateTime startDate, DateTime endDate, Domain.CoverType coverType);
    }
}
