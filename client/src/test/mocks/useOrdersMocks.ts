import { vi } from "vitest";
import type { UseMutationResult } from "@tanstack/react-query";
import type { PurchaseOrderDetails } from "@/features/orders/types";

/**
 * Returns a mocked TanStack Mutation result. Lets tests control
 * mutate behavior and pending state without spinning up the real mutation.
 */
export function createMockMutation(
  overrides: Partial<
    UseMutationResult<PurchaseOrderDetails, Error, unknown>
  > = {},
) {
  return {
    mutate: vi.fn(),
    mutateAsync: vi.fn(),
    isPending: false,
    isError: false,
    isSuccess: false,
    isIdle: true,
    error: null,
    data: undefined,
    reset: vi.fn(),
    variables: undefined,
    status: "idle",
    failureCount: 0,
    failureReason: null,
    submittedAt: 0,
    context: undefined,
    isPaused: false,
    ...overrides,
  } as unknown as UseMutationResult<PurchaseOrderDetails, Error, unknown>;
}
