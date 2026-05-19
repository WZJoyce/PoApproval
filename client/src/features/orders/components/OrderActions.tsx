import { useState } from "react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { ApiError } from "@/lib/apiClient";
import { useActingUser } from "@/features/auth/actingUser";
import {
  useApproveOrder,
  useRejectOrder,
  useSubmitOrder,
} from "../hooks/useOrders";
import { PurchaseOrderStatus, type PurchaseOrderDetails } from "../types";

interface OrderActionsProps {
  order: PurchaseOrderDetails;
}

/**
 * Converts an API error into a user-actionable message based on
 * HTTP status and the backend's RFC 7807 ruleCode extension.
 */
function describeError(error: unknown): string {
  if (error instanceof ApiError) {
    if (error.status === 409) {
      return "This order was modified by someone else. Refresh to see its current state.";
    }
    if (error.status === 422) {
      // Backend supplies a human-readable detail for business rule violations.
      return error.message;
    }
    return error.message;
  }
  return "An unexpected error occurred. Please try again.";
}

export function OrderActions({ order }: OrderActionsProps) {
  const { actingUser } = useActingUser();
  const submit = useSubmitOrder();
  const approve = useApproveOrder();
  const reject = useRejectOrder();

  const [rejectReason, setRejectReason] = useState("");
  const [rejectDialogOpen, setRejectDialogOpen] = useState(false);

  const isBusy = submit.isPending || approve.isPending || reject.isPending;

  const handleSubmit = () => {
    submit.mutate(
      { id: order.id, actingUser },
      {
        onSuccess: () => toast.success("Order submitted for approval."),
        onError: (error) => toast.error(describeError(error)),
      },
    );
  };

  const handleApprove = () => {
    approve.mutate(
      { id: order.id, actingUser },
      {
        onSuccess: () => toast.success("Order approved."),
        onError: (error) => toast.error(describeError(error)),
      },
    );
  };

  const handleReject = () => {
    if (rejectReason.trim().length < 10) {
      toast.error("Rejection reason must be at least 10 characters.");
      return;
    }
    reject.mutate(
      { id: order.id, actingUser, reason: rejectReason.trim() },
      {
        onSuccess: () => {
          toast.success("Order rejected.");
          setRejectDialogOpen(false);
          setRejectReason("");
        },
        onError: (error) => toast.error(describeError(error)),
      },
    );
  };

  // Only Draft orders can be submitted; only Submitted orders can be approved/rejected.
  const canSubmit = order.status === PurchaseOrderStatus.Draft;
  const canReview = order.status === PurchaseOrderStatus.Submitted;

  if (!canSubmit && !canReview) {
    return (
      <p className="text-sm text-muted-foreground">
        No actions available for an order in this state.
      </p>
    );
  }

  return (
    <div className="flex flex-wrap items-center gap-3">
      {canSubmit && (
        <Button onClick={handleSubmit} disabled={isBusy}>
          {submit.isPending ? "Submitting…" : "Submit for Approval"}
        </Button>
      )}

      {canReview && (
        <>
          <AlertDialog>
            <Button variant="default" disabled={isBusy} onClick={handleApprove}>
              {approve.isPending ? "Approving…" : "Approve"}
            </Button>
            {/* Approve is intentionally a direct action with a toast,
                not a dialog — it's a positive action with low blast radius
                and the self-approval rule is enforced server-side. */}
          </AlertDialog>

          <AlertDialog
            open={rejectDialogOpen}
            onOpenChange={setRejectDialogOpen}
          >
            <Button
              variant="destructive"
              disabled={isBusy}
              onClick={() => setRejectDialogOpen(true)}
            >
              Reject
            </Button>
            <AlertDialogContent>
              <AlertDialogHeader>
                <AlertDialogTitle>Reject this order?</AlertDialogTitle>
                <AlertDialogDescription>
                  This action cannot be undone directly. The creator will need
                  to resubmit. Please provide a clear reason.
                </AlertDialogDescription>
              </AlertDialogHeader>
              <Textarea
                value={rejectReason}
                onChange={(e) => setRejectReason(e.target.value)}
                placeholder="Reason for rejection (minimum 10 characters)…"
                rows={4}
              />
              <AlertDialogFooter>
                <AlertDialogCancel
                  onClick={() => {
                    setRejectReason("");
                  }}
                >
                  Cancel
                </AlertDialogCancel>
                <AlertDialogAction
                  onClick={(e) => {
                    e.preventDefault(); // prevent dialog auto-close before validation
                    handleReject();
                  }}
                  disabled={reject.isPending}
                >
                  {reject.isPending ? "Rejecting…" : "Confirm Reject"}
                </AlertDialogAction>
              </AlertDialogFooter>
            </AlertDialogContent>
          </AlertDialog>
        </>
      )}
    </div>
  );
}
