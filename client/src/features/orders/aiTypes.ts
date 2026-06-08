/**
 * Mirrors the backend AdvisorRecommendation (PoApproval.Domain.Advisory).
 * AI advisory output — advisory only, never an automated decision.
 */
export type AdvisorVerdict =
  | "LikelyApprove"
  | "ReviewCarefully"
  | "Investigate";
export type AdvisorSeverity = "Low" | "Medium" | "High";

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
