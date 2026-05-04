namespace PoApproval.Api.Configuration;

/// <summary>
/// Metadata exposed in the generated OpenAPI document.
/// </summary>
public sealed class OpenApiOptions
{
    public const string SectionName = "OpenApi";

    public string Title { get; init; } = "PoApproval API";
    public string Description { get; init; } = string.Empty;
    public ContactInfo Contact { get; init; } = new();

    public sealed class ContactInfo
    {
        public string Name { get; init; } = string.Empty;
        public string Url { get; init; } = string.Empty;
    }
}
