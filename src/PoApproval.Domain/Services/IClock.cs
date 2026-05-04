namespace PoApproval.Domain.Services;

/// <summary>
/// Provides the current UTC time. 
/// </summary>
public interface IClock
{
    DateTime UtcNow { get; }
}
