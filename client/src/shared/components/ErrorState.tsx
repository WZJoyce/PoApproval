import { ApiError } from "@/lib/apiClient";

interface ErrorStateProps {
  error: unknown;
  title?: string;
}

export function ErrorState({
  error,
  title = "Something went wrong",
}: ErrorStateProps) {
  const message =
    error instanceof ApiError
      ? `${error.status}: ${error.message}`
      : "An unexpected error occurred.";

  return (
    <div
      className="rounded-md border border-danger/30 bg-danger/10 p-4 text-sm"
      role="alert"
    >
      <p className="font-medium text-danger">{title}</p>
      <p className="mt-1 text-muted-foreground">{message}</p>
    </div>
  );
}
