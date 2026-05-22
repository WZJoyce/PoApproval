interface EmptyStateProps {
  message?: string;
}

export function EmptyState({ message = "No data found." }: EmptyStateProps) {
  return <p className="py-8 text-center text-muted-foreground">{message}</p>;
}
