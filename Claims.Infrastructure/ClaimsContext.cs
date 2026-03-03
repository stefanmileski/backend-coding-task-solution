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

        public async Task<Claim?> GetClaimAsync(Guid uid)
        {
            return await Claims
                .Where(claim => claim.Uid == uid)
                .SingleOrDefaultAsync();
        }

        public async Task AddClaimAsync(Claim claim)
        {
            Claims.Add(claim);
            await SaveChangesAsync();

            _auditer.AuditClaim(claim.Uid.ToString(), "POST");
        }

        public async Task DeleteClaimAsync(Guid uid)
        {
            Claim? claim = await GetClaimAsync(uid);
            if (claim is not null)
            {
                Claims.Remove(claim);
                await SaveChangesAsync();

                _auditer.AuditClaim(uid.ToString(), "DELETE");
            }
        }

        public async Task<IEnumerable<Cover>> GetCoversAsync()
        {
            return await Covers.ToListAsync();
        }

        public async Task<Cover?> GetCoverAsync(Guid uid)
        {
            return await Covers
                .Where(cover => cover.Uid == uid)
                .SingleOrDefaultAsync();
        }

        public async Task AddCoverAsync(Cover cover)
        {
            Covers.Add(cover);
            await SaveChangesAsync();

            _auditer.AuditCover(cover.Uid.ToString(), "POST");
        }

        public async Task DeleteCoverAsync(Guid uid)
        {
            Cover? cover = await GetCoverAsync(uid);
            if (cover is not null)
            {
                Covers.Remove(cover);
                await SaveChangesAsync();

                _auditer.AuditCover(uid.ToString(), "DELETE");
            }
        }
    }
}
