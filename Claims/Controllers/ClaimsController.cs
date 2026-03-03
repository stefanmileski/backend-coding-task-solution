using Claims.Auditing;
using Claims.Domain.Claim;
using Claims.Domain.Cover;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController(ClaimsContext claimsContext, AuditContext auditContext) : ControllerBase
    {
        private readonly ClaimsContext _claimsContext = claimsContext;
        private readonly Auditer _auditer = new(auditContext);

        [HttpGet]
        public async Task<IEnumerable<Claim>> GetAsync()
        {
            return await _claimsContext.GetClaimsAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(Claim claim)
        {
            claim.Id = Guid.NewGuid().ToString();
            await _claimsContext.AddItemAsync(claim);
            _auditer.AuditClaim(claim.Id, "POST");
            return Ok(claim);
        }

        [HttpDelete("{id}")]
        public async Task DeleteAsync(string id)
        {
            _auditer.AuditClaim(id, "DELETE");
            await _claimsContext.DeleteItemAsync(id);
        }

        [HttpGet("{id}")]
        public async Task<Claim> GetAsync(string id)
        {
            return await _claimsContext.GetClaimAsync(id);
        }
    }

    public class ClaimsContext(DbContextOptions options) : DbContext(options)
    {
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

        public async Task<Claim> GetClaimAsync(string id)
        {
            return await Claims
                .Where(claim => claim.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task AddItemAsync(Claim item)
        {
            Claims.Add(item);
            await SaveChangesAsync();
        }

        public async Task DeleteItemAsync(string id)
        {
            var claim = await GetClaimAsync(id);
            if (claim is not null)
            {
                Claims.Remove(claim);
                await SaveChangesAsync();
            }
        }
    }
}
