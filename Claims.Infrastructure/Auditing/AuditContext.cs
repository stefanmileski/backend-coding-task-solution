using Claims.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Claims.Infrastructure.Auditing
{
    public class AuditContext(DbContextOptions<AuditContext> options) : DbContext(options)
    {
        public DbSet<ClaimAudit> ClaimAudits { get; set; }
        public DbSet<CoverAudit> CoverAudits { get; set; }
    }
}
