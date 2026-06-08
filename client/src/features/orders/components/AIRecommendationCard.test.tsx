import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, it, expect, beforeEach, vi } from "vitest";

vi.mock("../hooks/useOrders", () => ({
  useAIRecommendation: vi.fn(),
}));

import { AIRecommendationCard } from "./AIRecommendationCard";
import { useAIRecommendation } from "../hooks/useOrders";

function mockHook(overrides = {}) {
  return {
    data: undefined,
    isFetching: false,
    isError: false,
    refetch: vi.fn(),
    ...overrides,
  } as unknown as ReturnType<typeof useAIRecommendation>;
}

describe("AIRecommendationCard", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("shows the get-recommendation button initially", () => {
    vi.mocked(useAIRecommendation).mockReturnValue(mockHook());

    render(<AIRecommendationCard orderId={1} />);

    expect(
      screen.getByRole("button", { name: /get recommendation/i }),
    ).toBeInTheDocument();
  });

  it("triggers refetch when the button is clicked", async () => {
    const user = userEvent.setup();
    const refetch = vi.fn();
    vi.mocked(useAIRecommendation).mockReturnValue(mockHook({ refetch }));

    render(<AIRecommendationCard orderId={1} />);
    await user.click(
      screen.getByRole("button", { name: /get recommendation/i }),
    );

    expect(refetch).toHaveBeenCalledOnce();
  });

  it("shows loading state while fetching", () => {
    vi.mocked(useAIRecommendation).mockReturnValue(
      mockHook({ isFetching: true }),
    );

    render(<AIRecommendationCard orderId={1} />);

    expect(screen.getByText(/analyzing/i)).toBeInTheDocument();
  });

  it("shows graceful degradation when unavailable", () => {
    vi.mocked(useAIRecommendation).mockReturnValue(
      mockHook({
        data: {
          verdict: "ReviewCarefully",
          confidence: 0,
          summary: "unavailable",
          flags: [],
          questionsForReviewer: [],
          isAvailable: false,
        },
      }),
    );

    render(<AIRecommendationCard orderId={1} />);

    expect(screen.getByText(/temporarily unavailable/i)).toBeInTheDocument();
  });

  it("renders verdict, confidence, and flags on success", () => {
    vi.mocked(useAIRecommendation).mockReturnValue(
      mockHook({
        data: {
          verdict: "Investigate",
          confidence: 0.75,
          summary: "Amount is unusually high.",
          flags: [
            { type: "AMOUNT_OUTLIER", severity: "High", detail: "33x average" },
          ],
          questionsForReviewer: ["Is this a one-time expense?"],
          isAvailable: true,
        },
      }),
    );

    render(<AIRecommendationCard orderId={1} />);

    expect(screen.getByText(/investigate/i)).toBeInTheDocument();
    expect(screen.getByText(/75%/)).toBeInTheDocument();
    expect(screen.getByText(/amount_outlier/i)).toBeInTheDocument();
    expect(screen.getByText(/one-time expense/i)).toBeInTheDocument();
  });
});
