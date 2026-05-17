import { Link, useParams } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { ApiError } from "@/lib/apiClient";
import { useOrder } from "../hooks/useOrders";
import { StatusBadge } from "../components/StatusBadge";

export function OrderDetailPage() {
  const { id: idParam } = useParams<{ id: string }>();
  const id = Number(idParam);

  const { data, isLoading, error } = useOrder(id);

  return (
    <div className="space-y-4">
      <div className="flex items-center gap-3">
        <Button variant="outline" size="sm" asChild>
          <Link to="/">← Back to list</Link>
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center justify-between">
            <span>{data?.orderNo ?? "Loading…"}</span>
            {data && <StatusBadge status={data.status} />}
          </CardTitle>
        </CardHeader>
        <CardContent>
          {isLoading && <p className="text-muted-foreground">Loading order…</p>}

          {error && (
            <div className="rounded-md border border-danger/30 bg-danger/10 p-4 text-sm">
              <p className="font-medium text-danger">Failed to load order.</p>
              <p className="mt-1 text-muted-foreground">
                {error instanceof ApiError
                  ? `${error.status}: ${error.message}`
                  : "Unknown error"}
              </p>
            </div>
          )}

          {data && (
            <dl className="grid grid-cols-2 gap-x-6 gap-y-4 text-sm">
              <div>
                <dt className="text-muted-foreground">Amount</dt>
                <dd className="mt-1 font-medium">${data.amount.toFixed(2)}</dd>
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
          )}
        </CardContent>
      </Card>
    </div>
  );
}
