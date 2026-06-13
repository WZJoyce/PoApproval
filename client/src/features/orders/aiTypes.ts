/**
 * Mirrors the backend AdvisorRecommendation (PoApproval.Domain.Advisory).
 * AI advisory output — advisory only, never an automated decision.
 */

export const AdvisorVerdict = {
  LikelyApprove: 0,
  ReviewCarefully: 1,
  Investigate: 2,
} as const;

export type AdvisorVerdict =
  (typeof AdvisorVerdict)[keyof typeof AdvisorVerdict];

export const VerdictLabel: Record<AdvisorVerdict, string> = {
  [AdvisorVerdict.LikelyApprove]: "Likely Approve",
  [AdvisorVerdict.ReviewCarefully]: "Review Carefully",
  [AdvisorVerdict.Investigate]: "Investigate",
};

export const AdvisorSeverity = {
  Low: 0,
  Medium: 1,
  High: 2,
} as const;

export type AdvisorSeverity =
  (typeof AdvisorSeverity)[keyof typeof AdvisorSeverity];

export const SeverityLabel: Record<AdvisorSeverity, string> = {
  [AdvisorSeverity.Low]: "LOW",
  [AdvisorSeverity.Medium]: "MEDIUM",
  [AdvisorSeverity.High]: "HIGH",
};

export interface AdvisorFlag {
  type: string;
  severity: AdvisorSeverity;
  detail: string;
}

export interface AdvisorRecommendation {
  verdict: AdvisorVerdict;
  confidence: number;
  summary: string;
  flags: AdvisorFlag[];
  questionsForReviewer: string[];
  isAvailable: boolean;
}
