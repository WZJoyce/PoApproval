import { vi } from "vitest";
import type { UseMutationResult } from "@tanstack/react-query";

/**
 * Returns a mocked TanStack Mutation result. Lets tests control
 * mutate behavior and pending state without spinning up the real mutation.
 */
// eslint-disable-next-line @typescript-eslint/no-explicit-any
type AnyMutationResult = UseMutationResult<any, any, any, any>;

export function createMockMutation(
  overrides: Partial<AnyMutationResult> = {},
): AnyMutationResult {
  const base = {
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
    status: "idle" as const,
    failureCount: 0,
    failureReason: null,
    submittedAt: 0,
    context: undefined,
    isPaused: false,
    ...overrides,
  };

  // Double assertion — bypasses the discriminated-union narrowing
  // that would otherwise demand status-specific field combinations.
  return base as unknown as AnyMutationResult;
}
