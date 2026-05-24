import { vi } from "vitest";

export const toastMock = {
  success: vi.fn(),
  error: vi.fn(),
};

vi.mock("sonner", () => ({
  toast: toastMock,
}));
