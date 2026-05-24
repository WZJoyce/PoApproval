import { vi } from "vitest";

// Mock the hooks module — every import of useSubmit/Approve/RejectOrder
// inside OrderActions will receive the vi.fn() below.
vi.mock("../hooks/useOrders", () => ({
  useSubmitOrder: vi.fn(),
  useApproveOrder: vi.fn(),
  useRejectOrder: vi.fn(),
}));

// Mock sonner toast (the actual mock is defined in toast.ts)
import "@/test/mocks/toast";
