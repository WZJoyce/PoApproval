using FluentAssertions;
using Moq;
using PoApproval.Domain.Entities;
using PoApproval.Domain.Enums;
using PoApproval.Domain.Exceptions;
using PoApproval.Domain.Services;

namespace PoApproval.Domain.Tests.Services;

public partial class ApprovalServiceTests
{
    private readonly Mock<IClock> _clockMock = new();
    private readonly DateTime _now = new(2026, 5, 5, 10, 0, 0, DateTimeKind.Utc);
    private readonly IApprovalService _sut;

    public ApprovalServiceTests()
    {
        _clockMock.Setup(c => c.UtcNow).Returns(_now);
        _sut = new ApprovalService(_clockMock.Object);
    }

    private PurchaseOrder NewOrder(
        PurchaseOrderStatus status = PurchaseOrderStatus.Draft,
        decimal amount = 500m,
        string createdBy = "alice")
        => new()
        {
            Id = 1,
            OrderNo = "PO-001",
            Amount = amount,
            CreatedBy = createdBy,
            CreatedAt = _now.AddDays(-1),
            Status = status
        };

    [Fact]
    public void Submit_FromDraft_TransitionsToSubmitted()
    {
        var order = NewOrder(status: PurchaseOrderStatus.Draft);

        _sut.Submit(order);

        order.Status.Should().Be(PurchaseOrderStatus.Submitted);
    }

    [Theory]
    [InlineData(PurchaseOrderStatus.Submitted)]
    [InlineData(PurchaseOrderStatus.Approved)]
    [InlineData(PurchaseOrderStatus.Rejected)]
    public void Submit_FromNonDraftStatus_ThrowsInvalidStateTransition(PurchaseOrderStatus current)
    {
        var order = NewOrder(status: current);

        var act = () => _sut.Submit(order);

        var ex = act.Should()
            .Throw<InvalidStateTransitionException>()
            .Which;

        ex.CurrentStatus.Should().Be(current);
    }

    [Fact]
    public void Submit_DoesNotMutateOtherFields()
    {
        var order = NewOrder(status: PurchaseOrderStatus.Draft);
        var originalCreatedBy = order.CreatedBy;
        var originalAmount = order.Amount;

        _sut.Submit(order);

        order.CreatedBy.Should().Be(originalCreatedBy);
        order.Amount.Should().Be(originalAmount);
        order.ApprovedBy.Should().BeNull();
        order.ApprovedAt.Should().BeNull();
        order.RejectionReason.Should().BeNull();
    }
}
