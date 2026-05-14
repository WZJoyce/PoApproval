import { apiClient } from "@/lib/apiClient";
import type {
  PagedResponse,
  PurchaseOrderSummary,
  PurchaseOrderStatus,
} from "../types";

export interface ListOrdersParams {
  status?: PurchaseOrderStatus;
  page?: number;
  pageSize?: number;
}

export const ordersApi = {
  list: async (
    params: ListOrdersParams = {},
  ): Promise<PagedResponse<PurchaseOrderSummary>> => {
    const { data } = await apiClient.get<PagedResponse<PurchaseOrderSummary>>(
      "/api/v1/orders",
      { params },
    );
    return data;
  },
};
