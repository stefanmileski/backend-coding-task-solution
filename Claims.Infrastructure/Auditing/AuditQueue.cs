using Claims.Infrastructure.Auditing.Interfaces;
using System.Threading.Channels;

namespace Claims.Infrastructure.Auditing
{
    public class AuditQueue: IAuditQueue
    {
        private readonly Channel<AuditMessage> _channel =
            Channel.CreateUnbounded<AuditMessage>(new UnboundedChannelOptions
            {
                SingleReader = true
            });

        public void Enqueue(AuditMessage message) =>
            _channel.Writer.TryWrite(message);

        public ChannelReader<AuditMessage> Reader => _channel.Reader;
    }
}