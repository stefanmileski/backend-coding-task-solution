using Claims.Auditing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Claims.Infrastructure.Auditing
{
    public class AuditWorker(AuditQueue queue, IServiceScopeFactory scopeFactory): BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (AuditMessage message in queue.Reader.ReadAllAsync(stoppingToken))
            {
                await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
                AuditContext db = scope.ServiceProvider.GetRequiredService<AuditContext>();

                switch (message)
                {
                    case ClaimAuditMessage m:
                        db.ClaimAudits.Add(new ClaimAudit
                        {
                            ClaimId = m.Id,
                            HttpRequestType = m.HttpRequestType,
                            Created = DateTime.UtcNow
                        });
                        break;

                    case CoverAuditMessage m:
                        db.CoverAudits.Add(new CoverAudit
                        {
                            CoverId = m.Id,
                            HttpRequestType = m.HttpRequestType,
                            Created = DateTime.UtcNow
                        });
                        break;
                }

                await db.SaveChangesAsync(stoppingToken);
            }
        }
    }
}
