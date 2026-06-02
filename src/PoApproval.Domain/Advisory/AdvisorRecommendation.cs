namespace PoApproval.Domain.Advisory;

/// <summary>
/// AI-generated advisory output for a purchase order under review.
/// Advisory only — never an automated decision. The human reviewer decides.
/// </summary>
public sealed record AdvisorRecommendation
{
    public required AdvisorVerdict Verdict { get; init; }
    public required double Confidence { get; init; }
    public required string Summary { get; init; }
    public required IReadOnlyList<AdvisorFlag> Flags { get; init; }
    public required IReadOnlyList<string> QuestionsForReviewer { get; init; }

    /// <summary>
    /// Indicates the advisor could not produce a recommendation
    /// </summary>
    public bool IsAvailable { get; init; } = true;

    public static AdvisorRecommendation Unavailable() => new()
    {
        Verdict = AdvisorVerdict.ReviewCarefully,
        Confidence = 0,
        Summary = "AI advisor is temporarily unavailable.",
        Flags = [],
        QuestionsForReviewer = [],
        IsAvailable = false,
    };
}

public sealed record AdvisorFlag
{
    public required string Type { get; init; }
    public required AdvisorSeverity Severity { get; init; }
    public required string Detail { get; init; }
}

public enum AdvisorVerdict
{
    LikelyApprove,
    ReviewCarefully,
    Investigate,
}

public enum AdvisorSeverity
{
    Low,
    Medium,
    High,
}
