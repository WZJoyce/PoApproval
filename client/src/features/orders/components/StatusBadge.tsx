import { Badge } from "@/components/ui/badge";
import { PurchaseOrderStatus, StatusLabel } from "../types";

interface StatusBadgeProps {
  status: PurchaseOrderStatus;
}

const statusStyles: Record<PurchaseOrderStatus, string> = {
  [PurchaseOrderStatus.Draft]: "bg-muted text-muted-foreground hover:bg-muted",
  [PurchaseOrderStatus.Submitted]:
    "bg-warning/20 text-warning-foreground hover:bg-warning/20",
  [PurchaseOrderStatus.Approved]:
    "bg-success/20 text-success-foreground hover:bg-success/20",
  [PurchaseOrderStatus.Rejected]:
    "bg-danger/20 text-danger-foreground hover:bg-danger/20",
};

export function StatusBadge({ status }: StatusBadgeProps) {
  return (
    <Badge variant="outline" className={statusStyles[status]}>
      {StatusLabel[status]}
    </Badge>
  );
}
