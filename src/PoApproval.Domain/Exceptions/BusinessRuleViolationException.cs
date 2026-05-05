namespace PoApproval.Domain.Exceptions;

/// <summary>
/// Thrown when an operation is rejected because a business rule is violated,
/// independent of the entity's lifecycle state.
/// </summary>
public sealed class BusinessRuleViolationException : Exception
{
    public string RuleCode { get; }

    public BusinessRuleViolationException(string ruleCode, string message)
        : base(message)
    {
        RuleCode = ruleCode;
    }
}
