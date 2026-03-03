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

        public async Task AddItemAsync(Claim claim)
        {
            Claims.Add(claim);
            await SaveChangesAsync();

            _auditer.AuditClaim(claim.Uid.ToString(), "POST");
        }

        public async Task DeleteItemAsync(Guid uid)
        {
            Claim? claim = await GetClaimAsync(uid);
            if (claim is not null)
            {
                Claims.Remove(claim);
                await SaveChangesAsync();

                _auditer.AuditClaim(uid.ToString(), "DELETE");
            }
        }
    }
}
