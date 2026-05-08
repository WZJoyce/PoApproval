using FluentAssertions;
using PoApproval.Domain.Enums;
using PoApproval.Domain.Exceptions;

namespace PoApproval.Domain.Tests.Services;

public partial class ApprovalServiceTests
{
    private const string _validRejectionReason = "Budget exceeded for this month.";

    [Fact]
    public void Reject_FromSubmitted_TransitionsToRejected()
    {
        var order = NewOrder(status: PurchaseOrderStatus.Submitted, createdBy: "alice");

        _sut.Reject(order, approver: "catherine", reason: _validRejectionReason);

        order.Status.Should().Be(PurchaseOrderStatus.Rejected);
        order.ReviewedBy.Should().Be("catherine");
        order.ReviewedAt.Should().Be(_now);
        order.RejectionReason.Should().Be(_validRejectionReason);
    }

    [Fact]
    public void Reject_BySameUserWhoCreated_IsAllowed()
    {
        // The creator may withdraw their own draft via reject (unlike approval).
        var order = NewOrder(status: PurchaseOrderStatus.Submitted, createdBy: "alice");

        var act = () => _sut.Reject(order, approver: "alice", reason: _validRejectionReason);

        act.Should().NotThrow();
        order.Status.Should().Be(PurchaseOrderStatus.Rejected);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Reject_WithBlankApprover_ThrowsBusinessRuleViolation(string approver)
    {
        var order = NewOrder(status: PurchaseOrderStatus.Submitted, createdBy: "alice");

        var act = () => _sut.Reject(order, approver, reason: _validRejectionReason);

        act.Should()
            .Throw<BusinessRuleViolationException>()
            .Which.RuleCode.Should().Be("APPROVER_REQUIRED");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("too short")]   // 9 chars, below the 10-char minimum
    public void Reject_WithInvalidReason_ThrowsBusinessRuleViolation(string reason)
    {
        var order = NewOrder(status: PurchaseOrderStatus.Submitted, createdBy: "alice");

        var act = () => _sut.Reject(order, approver: "catherine", reason);

        act.Should()
            .Throw<BusinessRuleViolationException>()
            .Which.RuleCode.Should().Be("REJECTION_REASON_INVALID");
    }

    [Theory]
    [InlineData(PurchaseOrderStatus.Draft)]
    [InlineData(PurchaseOrderStatus.Approved)]
    [InlineData(PurchaseOrderStatus.Rejected)]
    public void Reject_FromNonSubmittedStatus_ThrowsInvalidStateTransition(PurchaseOrderStatus current)
    {
        var order = NewOrder(status: current, createdBy: "alice");

        var act = () => _sut.Reject(order, approver: "catherine", reason: _validRejectionReason);

        act.Should()
            .Throw<InvalidStateTransitionException>()
            .Which.CurrentStatus.Should().Be(current);
    }

    [Fact]
    public void Reject_TrimsReasonWhitespace()
    {
        var order = NewOrder(status: PurchaseOrderStatus.Submitted, createdBy: "alice");
        const string reasonWithPadding = "  Budget exceeded for this quarter.  ";

        _sut.Reject(order, approver: "catherine", reason: reasonWithPadding);

        order.RejectionReason.Should().Be("Budget exceeded for this quarter.");
    }
}
