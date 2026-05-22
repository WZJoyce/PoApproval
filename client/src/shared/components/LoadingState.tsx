interface LoadingStateProps {
  message?: string;
}

export function LoadingState({ message = "Loading…" }: LoadingStateProps) {
  return (
    <p className="py-8 text-center text-muted-foreground" role="status">
      {message}
    </p>
  );
}
