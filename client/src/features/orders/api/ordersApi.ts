import { apiClient } from "@/lib/apiClient";
import type {
  PagedResponse,
  PurchaseOrderDetails,
  PurchaseOrderSummary,
  PurchaseOrderStatus,
} from "../types";
import type { AdvisorRecommendation } from "../aiTypes";

export interface ListOrdersParams {
  status?: PurchaseOrderStatus;
  page?: number;
  pageSize?: number;
}

export interface CreateOrderPayload {
  orderNo: string;
  amount: number;
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

  getById: async (id: number): Promise<PurchaseOrderDetails> => {
    const { data } = await apiClient.get<PurchaseOrderDetails>(
      `/api/v1/orders/${id}`,
    );
    return data;
  },

  create: async (
    payload: CreateOrderPayload,
    actingUser: string,
  ): Promise<PurchaseOrderDetails> => {
    const { data } = await apiClient.post<PurchaseOrderDetails>(
      "/api/v1/orders",
      payload,
      { headers: { "User-Id": actingUser } },
    );
    return data;
  },

  submit: async (
    id: number,
    actingUser: string,
  ): Promise<PurchaseOrderDetails> => {
    const { data } = await apiClient.post<PurchaseOrderDetails>(
      `/api/v1/orders/${id}/submit`,
      null,
      { headers: { "User-Id": actingUser } },
    );
    return data;
  },

  approve: async (
    id: number,
    actingUser: string,
  ): Promise<PurchaseOrderDetails> => {
    const { data } = await apiClient.post<PurchaseOrderDetails>(
      `/api/v1/orders/${id}/approve`,
      { approver: actingUser },
      { headers: { "User-Id": actingUser } },
    );
    return data;
  },

  reject: async (
    id: number,
    actingUser: string,
    reason: string,
  ): Promise<PurchaseOrderDetails> => {
    const { data } = await apiClient.post<PurchaseOrderDetails>(
      `/api/v1/orders/${id}/reject`,
      { approver: actingUser, reason },
      { headers: { "User-Id": actingUser } },
    );
    return data;
  },

  getAiRecommendation: async (id: number): Promise<AdvisorRecommendation> => {
    const { data } = await apiClient.get<AdvisorRecommendation>(
      `/api/v1/orders/${id}/ai-recommendation`,
    );
    return data;
  },
};
