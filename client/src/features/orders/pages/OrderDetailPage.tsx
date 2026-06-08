import { Link, useParams } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { LoadingState } from "@/shared/components/LoadingState";
import { ErrorState } from "@/shared/components/ErrorState";
import { AVAILABLE_USERS, useActingUser } from "@/features/auth/actingUser";
import { useOrder } from "../hooks/useOrders";
import { StatusBadge } from "../components/StatusBadge";
import { OrderActions } from "../components/OrderActions";
import { AIRecommendationCard } from "../components/AIRecommendationCard";
import { PurchaseOrderStatus } from "../types";

export function OrderDetailPage() {
  const { id: idParam } = useParams<{ id: string }>();
  const id = Number(idParam);

  const { data, isLoading, error } = useOrder(id);
  const { actingUser, setActingUser } = useActingUser();

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between gap-3">
        <Button variant="outline" size="sm" asChild>
          <Link to="/">← Back to list</Link>
        </Button>

        {/* Pre-auth temporary user switcher. Replaced by JWT identity in Week 4. */}
        <div className="flex items-center gap-2 text-sm">
          <span className="text-muted-foreground">Acting as:</span>
          <Select value={actingUser} onValueChange={setActingUser}>
            <SelectTrigger className="w-32">
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              {AVAILABLE_USERS.map((user) => (
                <SelectItem key={user} value={user}>
                  {user}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center justify-between">
            <span>{data?.orderNo ?? "Loading…"}</span>
            {data && <StatusBadge status={data.status} />}
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-6">
          {isLoading && <LoadingState message="Loading orders..." />}

          {error && <ErrorState error={error} title="Failed to load orders" />}

          {data && (
            <>
              <dl className="grid grid-cols-2 gap-x-6 gap-y-4 text-sm">
                <div>
                  <dt className="text-muted-foreground">Amount</dt>
                  <dd className="mt-1 font-medium">
                    ${data.amount.toFixed(2)}
                  </dd>
                </div>
                <div>
                  <dt className="text-muted-foreground">Created By</dt>
                  <dd className="mt-1 font-medium">{data.createdBy}</dd>
                </div>
                <div>
                  <dt className="text-muted-foreground">Created At</dt>
                  <dd className="mt-1 font-medium">
                    {new Date(data.createdAt).toLocaleString()}
                  </dd>
                </div>
                {data.reviewedBy && (
                  <div>
                    <dt className="text-muted-foreground">Reviewed By</dt>
                    <dd className="mt-1 font-medium">{data.reviewedBy}</dd>
                  </div>
                )}
                {data.reviewedAt && (
                  <div>
                    <dt className="text-muted-foreground">Reviewed At</dt>
                    <dd className="mt-1 font-medium">
                      {new Date(data.reviewedAt).toLocaleString()}
                    </dd>
                  </div>
                )}
                {data.rejectionReason && (
                  <div className="col-span-2">
                    <dt className="text-muted-foreground">Rejection Reason</dt>
                    <dd className="mt-1 rounded-md border border-border bg-muted/30 p-3 font-medium">
                      {data.rejectionReason}
                    </dd>
                  </div>
                )}
              </dl>
              {data.status === PurchaseOrderStatus.Submitted && (
                <div className="border-t border-border pt-6">
                  <AIRecommendationCard orderId={data.id} />
                </div>
              )}
              <div className="border-t border-border pt-6">
                <OrderActions order={data} />
              </div>
            </>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
