using FluentAssertions;
using PoApproval.Domain.Enums;
using PoApproval.Domain.Exceptions;

namespace PoApproval.Domain.Tests.Services;

public partial class ApprovalServiceTests
{
    [Fact]
    public void Approve_FromSubmittedByDifferentUser_TransitionsToApproved()
    {
        var order = NewOrder(status: PurchaseOrderStatus.Submitted, createdBy: "catherine");

        _sut.Approve(order, approver: "bob");

        order.Status.Should().Be(PurchaseOrderStatus.Approved);
        order.ReviewedBy.Should().Be("bob");
        order.ReviewedAt.Should().Be(_now);
    }

    [Fact]
    public void Approve_BySameUserWhoCreated_ThrowsBusinessRuleViolation()
    {
        var order = NewOrder(status: PurchaseOrderStatus.Submitted, createdBy: "catherine");

        var act = () => _sut.Approve(order, approver: "catherine");

        act.Should()
            .Throw<BusinessRuleViolationException>()
            .Which.RuleCode.Should().Be("SELF_APPROVAL_FORBIDDEN");
    }

    [Fact]
    public void Approve_BySameUserDifferentCasing_ThrowsBusinessRuleViolation()
    {
        // Usernames are case-insensitive.
        var order = NewOrder(status: PurchaseOrderStatus.Submitted, createdBy: "catherine");

        var act = () => _sut.Approve(order, approver: "catherine");

        act.Should().Throw<BusinessRuleViolationException>();
    }

    [Theory]
    [InlineData(PurchaseOrderStatus.Draft)]
    [InlineData(PurchaseOrderStatus.Approved)]
    [InlineData(PurchaseOrderStatus.Rejected)]
    public void Approve_FromNonSubmittedStatus_ThrowsInvalidStateTransition(PurchaseOrderStatus current)
    {
        var order = NewOrder(status: current, createdBy: "catherine");

        var act = () => _sut.Approve(order, approver: "bob");

        act.Should()
            .Throw<InvalidStateTransitionException>()
            .Which.CurrentStatus.Should().Be(current);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Approve_WithBlankApprover_ThrowsBusinessRuleViolation(string approver)
    {
        var order = NewOrder(status: PurchaseOrderStatus.Submitted, createdBy: "catherine");

        var act = () => _sut.Approve(order, approver);

        act.Should()
            .Throw<BusinessRuleViolationException>()
            .Which.RuleCode.Should().Be("APPROVER_REQUIRED");
    }
}
