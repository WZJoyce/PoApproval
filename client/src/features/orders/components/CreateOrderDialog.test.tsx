import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, it, expect, beforeEach, vi } from "vitest";
import "./_test.setup";

import { CreateOrderDialog } from "./CreateOrderDialog";
import { useCreateOrder } from "../hooks/useOrders";
import { ApiError } from "@/lib/apiClient";
import { WithActingUser } from "@/test/utils/withActingUser";
import { createMockMutation } from "@/test/mocks/useOrdersMocks";
import { toastMock } from "@/test/mocks/toast";

describe("CreateOrderDialog", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(useCreateOrder).mockReturnValue(createMockMutation());
  });

  it("opens the dialog when New Order is clicked", async () => {
    const user = userEvent.setup();

    render(
      <WithActingUser>
        <CreateOrderDialog />
      </WithActingUser>,
    );

    await user.click(screen.getByRole("button", { name: /new order/i }));

    expect(
      await screen.findByRole("dialog", { name: /create new order/i }),
    ).toBeInTheDocument();
  });

  it("shows validation errors when OrderNo is too short", async () => {
    const user = userEvent.setup();
    const createMutation = createMockMutation();
    vi.mocked(useCreateOrder).mockReturnValue(createMutation);

    render(
      <WithActingUser>
        <CreateOrderDialog />
      </WithActingUser>,
    );

    await user.click(screen.getByRole("button", { name: /new order/i }));
    await user.type(await screen.findByLabelText(/order number/i), "AB");
    await user.type(screen.getByLabelText(/amount/i), "100");
    await user.click(screen.getByRole("button", { name: /^create$/i }));

    expect(
      await screen.findByText(/at least 3 characters/i),
    ).toBeInTheDocument();
    expect(createMutation.mutate).not.toHaveBeenCalled();
  });

  it("calls create mutation with valid values", async () => {
    const user = userEvent.setup();
    const createMutation = createMockMutation();
    vi.mocked(useCreateOrder).mockReturnValue(createMutation);

    render(
      <WithActingUser user="alice">
        <CreateOrderDialog />
      </WithActingUser>,
    );

    await user.click(screen.getByRole("button", { name: /new order/i }));
    await user.type(
      await screen.findByLabelText(/order number/i),
      "PO-2026-001",
    );
    await user.type(screen.getByLabelText(/amount/i), "1500");
    await user.click(screen.getByRole("button", { name: /^create$/i }));

    expect(createMutation.mutate).toHaveBeenCalledWith(
      {
        payload: { orderNo: "PO-2026-001", amount: 1500 },
        actingUser: "alice",
      },
      expect.objectContaining({
        onSuccess: expect.any(Function),
        onError: expect.any(Function),
      }),
    );
  });

  it("translates 409 conflict into a duplicate-orderno message", async () => {
    const user = userEvent.setup();
    const createMutation = createMockMutation({
      mutate: vi.fn((_vars, options) => {
        options?.onError?.(new ApiError("Conflict", 409, null));
      }),
    });
    vi.mocked(useCreateOrder).mockReturnValue(createMutation);

    render(
      <WithActingUser>
        <CreateOrderDialog />
      </WithActingUser>,
    );

    await user.click(screen.getByRole("button", { name: /new order/i }));
    await user.type(
      await screen.findByLabelText(/order number/i),
      "PO-DUP-001",
    );
    await user.type(screen.getByLabelText(/amount/i), "100");
    await user.click(screen.getByRole("button", { name: /^create$/i }));

    expect(toastMock.error).toHaveBeenCalledWith(
      expect.stringMatching(/already exists/i),
    );
  });
});
