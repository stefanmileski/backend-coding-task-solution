using Claims.Infrastructure.Auditing.Interfaces;

namespace Claims.Infrastructure.Auditing
{
    public class Auditer(IAuditQueue _queue): IAuditer
    {
        public void AuditClaim(string id, string httpRequestType) =>
            _queue.Enqueue(new ClaimAuditMessage(id, httpRequestType));

        public void AuditCover(string id, string httpRequestType) =>
            _queue.Enqueue(new CoverAuditMessage(id, httpRequestType));
    }
}
