using Claims.Infrastructure.Auditing.Interfaces;

namespace Claims.Infrastructure.Auditing
{
    public class Auditer(AuditQueue queue): IAuditer
    {
        private readonly AuditQueue _queue = queue;

        public void AuditClaim(string id, string httpRequestType) =>
            _queue.Enqueue(new ClaimAuditMessage(id, httpRequestType));

        public void AuditCover(string id, string httpRequestType) =>
            _queue.Enqueue(new CoverAuditMessage(id, httpRequestType));
    }
}
