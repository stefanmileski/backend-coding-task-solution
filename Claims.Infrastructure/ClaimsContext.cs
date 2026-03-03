using Claims.Auditing;
using Claims.Domain;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Infrastructure
{
    public class ClaimsContext(DbContextOptions options, AuditContext auditContext) : DbContext(options)
    {
        private readonly Auditer _auditer = new(auditContext);

        private DbSet<Claim> Claims { get; init; }
        public DbSet<Cover> Covers { get; init; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Claim>().ToCollection("claims");
            modelBuilder.Entity<Cover>().ToCollection("covers");
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

        public async Task<string> AddClaimAsync(Claim claim)
        {
            Claims.Add(claim);
            await SaveChangesAsync();

            _auditer.AuditClaim(claim.Id, "POST");

            return claim.Id;
        }

        public async Task<bool> DeleteClaimAsync(string id)
        {
            Claim? claim = await GetClaimAsync(id);
            if (claim is not null)
            {
                Claims.Remove(claim);
                await SaveChangesAsync();

                _auditer.AuditClaim(id, "DELETE");

                return true;
            }

            return false;
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

        public async Task<string> AddCoverAsync(Cover cover)
        {
            Covers.Add(cover);
            await SaveChangesAsync();

            _auditer.AuditCover(cover.Id, "POST");

            return cover.Id;
        }

        public async Task<bool> DeleteCoverAsync(string id)
        {
            Cover? cover = await GetCoverAsync(id);
            if (cover is not null)
            {
                Covers.Remove(cover);
                await SaveChangesAsync();

                _auditer.AuditCover(id, "DELETE");

                return true;
            }

            return false;
        }
    }
}
