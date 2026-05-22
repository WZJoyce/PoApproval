/**
 * Lives in shared/ because every API call (across all features)
 * can receive this on failure — it is not specific to orders.
 */
export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  ruleCode?: string;
  currentStatus?: string;
  attemptedTransition?: string;
}
