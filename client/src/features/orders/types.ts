/**
 * Lifecycle states of a purchase order.
 * Values must match the backend enum (PoApproval.Domain.Enums.PurchaseOrderStatus).
 */
export const PurchaseOrderStatus = {
    Draft: 0,
    Submitted: 1,
    Approved: 2,
    Rejected: 3,
} as const;

export type PurchaseOrderStatus = typeof PurchaseOrderStatus[keyof typeof PurchaseOrderStatus];

export const StatusLabel: Record<PurchaseOrderStatus, string> = {
    [PurchaseOrderStatus.Draft]: 'Draft',
    [PurchaseOrderStatus.Submitted]: 'Submitted',
    [PurchaseOrderStatus.Approved]: 'Approved',
    [PurchaseOrderStatus.Rejected]: 'Rejected',
};
  
  export interface PurchaseOrderSummary {
    id: number;
    orderNo: string;
    amount: number;
    status: PurchaseOrderStatus;
    createdBy: string;
    createdAt: string;
  }
  
  export interface PurchaseOrderDetails extends PurchaseOrderSummary {
    reviewedBy: string | null;
    reviewedAt: string | null;
    rejectionReason: string | null;
  }
  
  export interface PagedResponse<T> {
    items: T[];
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
    hasMore: boolean;
  }
  
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