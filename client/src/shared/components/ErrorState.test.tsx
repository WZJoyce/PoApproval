import { render, screen } from "@testing-library/react";
import { describe, it, expect } from "vitest";
import { ErrorState } from "./ErrorState";
import { ApiError } from "@/lib/apiClient";

describe("ErrorState", () => {
  it("renders the default title when none is provided", () => {
    render(<ErrorState error={new Error("boom")} />);

    expect(screen.getByText("Something went wrong")).toBeInTheDocument();
  });

  it("renders a custom title when provided", () => {
    render(
      <ErrorState error={new Error("boom")} title="Failed to load orders" />,
    );

    expect(screen.getByText("Failed to load orders")).toBeInTheDocument();
  });

  it("extracts status and message from an ApiError", () => {
    const apiError = new ApiError("Order not found", 404, null);

    render(<ErrorState error={apiError} />);

    expect(screen.getByText("404: Order not found")).toBeInTheDocument();
  });

  it("shows a generic message for non-ApiError errors", () => {
    render(<ErrorState error="just a string" />);

    expect(
      screen.getByText("An unexpected error occurred."),
    ).toBeInTheDocument();
  });

  it("exposes an alert role for screen readers", () => {
    render(<ErrorState error={new Error("boom")} />);

    expect(screen.getByRole("alert")).toBeInTheDocument();
  });
});
