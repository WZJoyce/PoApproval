import { Link, useSearchParams } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { LoadingState } from "@/shared/components/LoadingState";
import { ErrorState } from "@/shared/components/ErrorState";
import { EmptyState } from "@/shared/components/EmptyState";
import { useOrders } from "../hooks/useOrders";
import { PurchaseOrderStatus } from "../types";
import { StatusBadge } from "../components/StatusBadge";

const ALL_STATUSES = "all";

export function OrderListPage() {
  const [searchParams, setSearchParams] = useSearchParams();

  const statusParam = searchParams.get("status");
  const statusFilter = statusParam
    ? (Number(statusParam) as PurchaseOrderStatus)
    : undefined;

  const { data, isLoading, error, refetch, isFetching } = useOrders({
    status: statusFilter,
    page: 1,
    pageSize: 50,
  });

  const handleStatusChange = (value: string) => {
    if (value === ALL_STATUSES) {
      searchParams.delete("status");
    } else {
      searchParams.set("status", value);
    }
    setSearchParams(searchParams);
  };

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between gap-4">
        <CardTitle>Purchase Orders</CardTitle>
        <div className="flex items-center gap-3">
          <Select
            value={
              statusFilter !== undefined ? String(statusFilter) : ALL_STATUSES
            }
            onValueChange={handleStatusChange}
          >
            <SelectTrigger className="w-40">
              <SelectValue placeholder="Filter by status" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value={ALL_STATUSES}>All</SelectItem>
              <SelectItem value={String(PurchaseOrderStatus.Draft)}>
                Draft
              </SelectItem>
              <SelectItem value={String(PurchaseOrderStatus.Submitted)}>
                Submitted
              </SelectItem>
              <SelectItem value={String(PurchaseOrderStatus.Approved)}>
                Approved
              </SelectItem>
              <SelectItem value={String(PurchaseOrderStatus.Rejected)}>
                Rejected
              </SelectItem>
            </SelectContent>
          </Select>
          <Button
            variant="outline"
            onClick={() => refetch()}
            disabled={isFetching}
          >
            {isFetching ? "Refreshing…" : "Refresh"}
          </Button>
        </div>
      </CardHeader>
      <CardContent>
        {isLoading && <LoadingState message="Loading orders..." />}

        {error && <ErrorState error={error} title="Failed to load orders" />}

        {data && data.items.length === 0 && (
          <EmptyState message="No orders found." />
        )}

        {data && data.items.length > 0 && (
          <>
            <p className="mb-3 text-sm text-muted-foreground">
              Showing {data.items.length} of {data.totalCount} orders
            </p>
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Order No</TableHead>
                  <TableHead>Amount</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Created By</TableHead>
                  <TableHead>Created At</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {data.items.map((order) => (
                  <TableRow key={order.id} className="cursor-pointer">
                    <TableCell>
                      <Link
                        to={`/orders/${order.id}`}
                        className="font-medium text-primary hover:underline"
                      >
                        {order.orderNo}
                      </Link>
                    </TableCell>
                    <TableCell>${order.amount.toFixed(2)}</TableCell>
                    <TableCell>
                      <StatusBadge status={order.status} />
                    </TableCell>
                    <TableCell>{order.createdBy}</TableCell>
                    <TableCell>
                      {new Date(order.createdAt).toLocaleString()}
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </>
        )}
      </CardContent>
    </Card>
  );
}
