namespace Claims.Infrastructure.Auditing
{
    public abstract record AuditMessage(string Id, string HttpRequestType);
    public record ClaimAuditMessage(string Id, string HttpRequestType) : AuditMessage(Id, HttpRequestType);
    public record CoverAuditMessage(string Id, string HttpRequestType) : AuditMessage(Id, HttpRequestType);
}