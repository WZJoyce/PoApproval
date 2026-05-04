namespace PoApproval.Domain.Services;

/// <summary>
/// Production implementation of IClock, using the real system time.
/// </summary>
public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
