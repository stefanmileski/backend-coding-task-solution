using System.Threading.Channels;

namespace Claims.Infrastructure.Auditing.Interfaces
{
    public interface IAuditQueue
    {
        /// <summary>
        /// Enqueues an audit message to be processed later.
        /// </summary>
        /// <param name="message">The audit message to enqueue.</param>
        void Enqueue(AuditMessage message);

        /// <summary>
        /// Gets the channel reader for consuming audit messages.
        /// </summary>
        ChannelReader<AuditMessage> Reader { get; }
    }
}
