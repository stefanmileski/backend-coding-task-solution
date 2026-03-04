using Claims.Infrastructure.Auditing.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Claims.Infrastructure.Auditing
{
    public class AuditWorker(IAuditQueue _queue, IServiceScopeFactory scopeFactory): BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (AuditMessage message in _queue.Reader.ReadAllAsync(stoppingToken))
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
