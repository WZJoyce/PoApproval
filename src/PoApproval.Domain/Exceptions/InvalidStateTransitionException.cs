using PoApproval.Domain.Enums;

namespace PoApproval.Domain.Exceptions;

/// <summary>
/// Thrown when an operation attempts to transition a purchase order
/// from a state that does not allow it.
/// </summary>
public sealed class InvalidStateTransitionException : Exception
{
    public PurchaseOrderStatus CurrentStatus { get; }
    public PurchaseOrderStatus AttemptedTransition { get; }

    public InvalidStateTransitionException(
        PurchaseOrderStatus currentStatus,
        PurchaseOrderStatus attemptedTransition)
        : base($"Cannot transition from {currentStatus} to {attemptedTransition}.")
    {
        CurrentStatus = currentStatus;
        AttemptedTransition = attemptedTransition;
    }
}