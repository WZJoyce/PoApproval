import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, it, expect, beforeEach, vi } from "vitest";
import "./_test.setup";

import { OrderActions } from "./OrderActions";
import { PurchaseOrderStatus, type PurchaseOrderDetails } from "../types";
import {
  useSubmitOrder,
  useApproveOrder,
  useRejectOrder,
} from "../hooks/useOrders";
import { ApiError } from "@/lib/apiClient";
import { WithActingUser } from "@/test/utils/withActingUser";
import { createMockMutation } from "@/test/mocks/useOrdersMocks";
import { toastMock } from "@/test/mocks/toast";

function makeOrder(
  overrides: Partial<PurchaseOrderDetails> = {},
): PurchaseOrderDetails {
  return {
    id: 1,
    orderNo: "PO-TEST-001",
    amount: 1000,
    status: PurchaseOrderStatus.Draft,
    createdBy: "alice",
    createdAt: "2026-01-15T10:00:00Z",
    reviewedBy: null,
    reviewedAt: null,
    rejectionReason: null,
    ...overrides,
  };
}

describe("OrderActions", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    // Default: every mutation in idle state.
    vi.mocked(useSubmitOrder).mockReturnValue(createMockMutation());
    vi.mocked(useApproveOrder).mockReturnValue(createMockMutation());
    vi.mocked(useRejectOrder).mockReturnValue(createMockMutation());
  });

  it("shows Submit button when order is in Draft status", () => {
    render(
      <WithActingUser>
        <OrderActions
          order={makeOrder({ status: PurchaseOrderStatus.Draft })}
        />
      </WithActingUser>,
    );

    expect(
      screen.getByRole("button", { name: /submit for approval/i }),
    ).toBeInTheDocument();
    expect(
      screen.queryByRole("button", { name: /approve/i }),
    ).not.toBeInTheDocument();
  });

  it("shows Approve and Reject buttons when order is Submitted", () => {
    render(
      <WithActingUser>
        <OrderActions
          order={makeOrder({ status: PurchaseOrderStatus.Submitted })}
        />
      </WithActingUser>,
    );

    expect(
      screen.getByRole("button", { name: /^approve$/i }),
    ).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /reject/i })).toBeInTheDocument();
    expect(
      screen.queryByRole("button", { name: /submit for approval/i }),
    ).not.toBeInTheDocument();
  });

  it("shows nothing actionable when order is Approved", () => {
    render(
      <WithActingUser>
        <OrderActions
          order={makeOrder({ status: PurchaseOrderStatus.Approved })}
        />
      </WithActingUser>,
    );

    expect(screen.getByText(/no actions available/i)).toBeInTheDocument();
  });

  it("calls approve mutation with order id and acting user", async () => {
    const user = userEvent.setup();
    const approveMutation = createMockMutation();
    vi.mocked(useApproveOrder).mockReturnValue(approveMutation);

    render(
      <WithActingUser user="bob">
        <OrderActions
          order={makeOrder({
            id: 42,
            status: PurchaseOrderStatus.Submitted,
            createdBy: "alice",
          })}
        />
      </WithActingUser>,
    );

    await user.click(screen.getByRole("button", { name: /^approve$/i }));

    expect(approveMutation.mutate).toHaveBeenCalledWith(
      { id: 42, actingUser: "bob" },
      expect.objectContaining({
        onSuccess: expect.any(Function),
        onError: expect.any(Function),
      }),
    );
  });

  it("disables buttons while a mutation is pending", () => {
    vi.mocked(useApproveOrder).mockReturnValue(
      createMockMutation({ isPending: true }),
    );

    render(
      <WithActingUser>
        <OrderActions
          order={makeOrder({ status: PurchaseOrderStatus.Submitted })}
        />
      </WithActingUser>,
    );

    expect(screen.getByRole("button", { name: /approving/i })).toBeDisabled();
    expect(screen.getByRole("button", { name: /reject/i })).toBeDisabled();
  });

  it("rejects a too-short reason without calling the mutation", async () => {
    const user = userEvent.setup();
    const rejectMutation = createMockMutation();
    vi.mocked(useRejectOrder).mockReturnValue(rejectMutation);

    render(
      <WithActingUser>
        <OrderActions
          order={makeOrder({ status: PurchaseOrderStatus.Submitted })}
        />
      </WithActingUser>,
    );

    // Open dialog
    await user.click(screen.getByRole("button", { name: /reject/i }));

    // Type a too-short reason
    const textarea =
      await screen.findByPlaceholderText(/reason for rejection/i);
    await user.type(textarea, "too short");

    // Try to confirm
    await user.click(screen.getByRole("button", { name: /confirm reject/i }));

    expect(rejectMutation.mutate).not.toHaveBeenCalled();
    expect(toastMock.error).toHaveBeenCalledWith(
      expect.stringMatching(/at least 10 characters/i),
    );
  });

  it("translates an HTTP 409 error into a refresh-suggesting message", async () => {
    const user = userEvent.setup();

    // Configure the approve mutation to invoke onError synchronously
    // with a 409 ApiError, so we can verify the toast translation.
    const approveMutation = createMockMutation({
      mutate: vi.fn((_vars, options) => {
        options?.onError?.(new ApiError("Conflict", 409, null));
      }),
    });
    vi.mocked(useApproveOrder).mockReturnValue(approveMutation);

    render(
      <WithActingUser user="bob">
        <OrderActions
          order={makeOrder({ status: PurchaseOrderStatus.Submitted })}
        />
      </WithActingUser>,
    );

    await user.click(screen.getByRole("button", { name: /^approve$/i }));

    expect(toastMock.error).toHaveBeenCalledWith(
      expect.stringMatching(/modified by someone else.*refresh/i),
    );
  });
});
