import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { useAIRecommendation } from "../hooks/useOrders";
import { AdvisorVerdict, VerdictLabel } from "../aiTypes";
import { AdvisorSeverity, SeverityLabel } from "../aiTypes";

interface AIRecommendationCardProps {
  orderId: number;
}

const verdictStyles: Record<AdvisorVerdict, string> = {
  [AdvisorVerdict.LikelyApprove]: "bg-success/20 text-success-foreground",
  [AdvisorVerdict.ReviewCarefully]: "bg-danger/20 text-danger-foreground",
  [AdvisorVerdict.Investigate]: "bg-danger/20 text-danger-foreground",
};

const severityStyles: Record<AdvisorSeverity, string> = {
  [AdvisorSeverity.Low]: "border-border text-muted-foreground",
  [AdvisorSeverity.Medium]: "border-warning/50 text-warning-foreground",
  [AdvisorSeverity.High]: "border-danger/50 text-danger",
};

export function AIRecommendationCard({ orderId }: AIRecommendationCardProps) {
  const { data, isFetching, refetch, isError } = useAIRecommendation(orderId);

  // Initial state — user hasn't requested a recommendation yet.
  if (!data && !isFetching && !isError) {
    return (
      <div className="rounded-lg border border-border bg-muted/20 p-4">
        <div className="flex items-center justify-between gap-4">
          <div>
            <p className="text-sm font-medium">AI Approval Advisor</p>
            <p className="mt-1 text-sm text-muted-foreground">
              Get an AI-generated analysis based on historical patterns.
              Advisory only — the decision remains yours.
            </p>
          </div>
          <Button variant="outline" onClick={() => refetch()}>
            Get recommendation
          </Button>
        </div>
      </div>
    );
  }

  // Loading state.
  if (isFetching) {
    return (
      <div
        className="rounded-lg border border-border bg-muted/20 p-4"
        role="status"
      >
        <p className="text-sm font-medium">AI Approval Advisor</p>
        <p className="mt-1 text-sm text-muted-foreground">
          Analyzing order against historical patterns…
        </p>
      </div>
    );
  }

  // Error or unavailable — graceful degradation.
  if (isError || (data && !data.isAvailable)) {
    return (
      <div className="rounded-lg border border-border bg-muted/20 p-4">
        <div className="flex items-center justify-between gap-4">
          <div>
            <p className="text-sm font-medium">AI Approval Advisor</p>
            <p className="mt-1 text-sm text-muted-foreground">
              The AI advisor is temporarily unavailable. You can still review
              and approve this order manually.
            </p>
          </div>
          <Button variant="outline" onClick={() => refetch()}>
            Retry
          </Button>
        </div>
      </div>
    );
  }

  // Success — render the recommendation.
  if (!data) return null;

  const confidencePct = Math.round(
    Math.min(Math.max(data.confidence, 0), 1) * 100,
  );

  return (
    <div className="space-y-4 rounded-lg border border-border bg-muted/20 p-4">
      <div className="flex items-center justify-between gap-4">
        <p className="text-sm font-medium">AI Approval Advisor</p>
        <Badge variant="outline" className={verdictStyles[data.verdict]}>
          {VerdictLabel[data.verdict]}
        </Badge>
      </div>

      <p className="text-sm">{data.summary}</p>

      {/* Confidence bar */}
      <div>
        <div className="flex items-center justify-between text-xs text-muted-foreground">
          <span>Confidence</span>
          <span>{confidencePct}%</span>
        </div>
        <div className="mt-1 h-2 w-full overflow-hidden rounded-full bg-border">
          <div
            className="h-full rounded-full bg-primary transition-all"
            style={{ width: `${confidencePct}%` }}
          />
        </div>
      </div>

      {/* Flags */}
      {data.flags.length > 0 && (
        <div className="space-y-2">
          <p className="text-xs font-medium text-muted-foreground">Flags</p>
          {data.flags.map((flag, i) => (
            <div
              key={i}
              className={`rounded-md border px-3 py-2 text-sm ${severityStyles[flag.severity]}`}
            >
              <span className="font-medium">{flag.type}</span>
              <span className="ml-2 text-xs uppercase">
                ({SeverityLabel[flag.severity]})
              </span>
              <p className="mt-1 text-muted-foreground">{flag.detail}</p>
            </div>
          ))}
        </div>
      )}

      {/* Questions for reviewer */}
      {data.questionsForReviewer.length > 0 && (
        <div className="space-y-1">
          <p className="text-xs font-medium text-muted-foreground">
            Questions to consider
          </p>
          <ul className="list-inside list-disc space-y-1 text-sm text-muted-foreground">
            {data.questionsForReviewer.map((q, i) => (
              <li key={i}>{q}</li>
            ))}
          </ul>
        </div>
      )}

      <p className="text-xs text-muted-foreground">
        This is an AI-generated suggestion, not a decision. Use your judgment.
      </p>
    </div>
  );
}
