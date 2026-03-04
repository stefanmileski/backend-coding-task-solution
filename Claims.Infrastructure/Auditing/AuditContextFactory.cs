using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Claims.Infrastructure.Auditing
{
    public class AuditContextFactory : IDesignTimeDbContextFactory<AuditContext>
    {
        public AuditContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<AuditContext>()
                .UseSqlServer("Server=localhost;Database=AuditDb;Trusted_Connection=True;")
                .Options;

            return new AuditContext(options);
        }
    }
}
