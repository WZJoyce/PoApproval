import { useState } from 'react';
import { ApiError } from '@/lib/apiClient';
import { useOrders } from '../hooks/useOrders';
import { PurchaseOrderStatus, StatusLabel } from '../types';

export function OrderListPage() {
  const [statusFilter, setStatusFilter] = useState<PurchaseOrderStatus | undefined>(undefined);

  const { data, isLoading, error, refetch } = useOrders({
    status: statusFilter,
    page: 1,
    pageSize: 50,
  });

  return (
    <div style={{ padding: 24, fontFamily: 'system-ui, sans-serif' }}>
      <h1>Purchase Orders</h1>

      <div style={{ marginBottom: 16 }}>
        <label htmlFor="status-filter">Filter by status:&nbsp;</label>
        <select
          id="status-filter"
          value={statusFilter ?? ''}
          onChange={(e) =>
            setStatusFilter(
              e.target.value === ''
                ? undefined
                : (Number(e.target.value) as PurchaseOrderStatus)
            )
          }
        >
          <option value="">All</option>
          <option value={PurchaseOrderStatus.Draft}>Draft</option>
          <option value={PurchaseOrderStatus.Submitted}>Submitted</option>
          <option value={PurchaseOrderStatus.Approved}>Approved</option>
          <option value={PurchaseOrderStatus.Rejected}>Rejected</option>
        </select>

        <button onClick={() => refetch()} style={{ marginLeft: 12 }}>
          Refresh
        </button>
      </div>

      {isLoading && <p>Loading orders…</p>}

      {error && (
        <div style={{ color: 'crimson' }}>
          <p>Failed to load orders.</p>
          <p>
            {error instanceof ApiError
              ? `${error.status}: ${error.message}`
              : 'Unknown error'}
          </p>
        </div>
      )}

      {data && data.items.length === 0 && <p>No orders found.</p>}

      {data && data.items.length > 0 && (
        <>
          <p style={{ color: '#666' }}>
            Showing {data.items.length} of {data.totalCount} orders
          </p>
          <table style={{ borderCollapse: 'collapse', width: '100%' }}>
            <thead>
              <tr style={{ borderBottom: '2px solid #333', textAlign: 'left' }}>
                <th style={{ padding: 8 }}>Order No</th>
                <th style={{ padding: 8 }}>Amount</th>
                <th style={{ padding: 8 }}>Status</th>
                <th style={{ padding: 8 }}>Created By</th>
                <th style={{ padding: 8 }}>Created At</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map((order) => (
                <tr key={order.id} style={{ borderBottom: '1px solid #ddd' }}>
                  <td style={{ padding: 8 }}>{order.orderNo}</td>
                  <td style={{ padding: 8 }}>${order.amount.toFixed(2)}</td>
                  <td style={{ padding: 8 }}>{StatusLabel[order.status]}</td>
                  <td style={{ padding: 8 }}>{order.createdBy}</td>
                  <td style={{ padding: 8 }}>
                    {new Date(order.createdAt).toLocaleString()}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </>
      )}
    </div>
  );
}