namespace PoApproval.Domain.Repositories;

/// <summary>
/// A paged slice of results plus metadata for client-side pagination.
/// </summary>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasMore => Page < TotalPages;
}
