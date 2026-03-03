using Claims.Auditing;
using Claims.Domain;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Infrastructure
{
    public class ClaimsContext(DbContextOptions options, AuditContext auditContext) : DbContext(options)
    {
        private readonly Auditer _auditer = new(auditContext);
        private const string DELETE_METHOD = "DELETE";
        private const string POST_METHOD = "POST";
        private const string CLAIMS_COLLECTION_NAME = "claims";
        private const string COVERS_COLLECTION_NAME = "covers";

        private DbSet<Claim> Claims { get; init; }
        public DbSet<Cover> Covers { get; init; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Claim>().ToCollection(CLAIMS_COLLECTION_NAME);
            modelBuilder.Entity<Cover>().ToCollection(COVERS_COLLECTION_NAME);
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync()
        {
            return await Claims.ToListAsync();
        }

        public async Task<Claim?> GetClaimAsync(string id)
        {
            return await Claims
                .Where(claim => claim.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task AddClaimAsync(Claim claim)
        {
            Claims.Add(claim);
            await SaveChangesAsync();

            _auditer.AuditClaim(claim.Id, POST_METHOD);
        }

        public async Task DeleteClaimAsync(string id)
        {
            Claim? claim = await GetClaimAsync(id);
            if (claim is not null)
            {
                Claims.Remove(claim);
                await SaveChangesAsync();

                _auditer.AuditClaim(id, DELETE_METHOD);
            }
        }

        public async Task<IEnumerable<Cover>> GetCoversAsync()
        {
            return await Covers.ToListAsync();
        }

        public async Task<Cover?> GetCoverAsync(string id)
        {
            return await Covers
                .Where(cover => cover.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task AddCoverAsync(Cover cover)
        {
            Covers.Add(cover);
            await SaveChangesAsync();

            _auditer.AuditCover(cover.Id, POST_METHOD);
        }

        public async Task DeleteCoverAsync(string id)
        {
            Cover? cover = await GetCoverAsync(id);
            if (cover is not null)
            {
                Covers.Remove(cover);
                await SaveChangesAsync();

                _auditer.AuditCover(id, DELETE_METHOD);
            }
        }
    }
}
