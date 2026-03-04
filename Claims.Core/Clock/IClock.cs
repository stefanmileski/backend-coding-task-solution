namespace Claims.Core.Clock
{
    public interface IClock
    {
        DateTime UtcNow { get; }
    }

}
