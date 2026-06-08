import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  ordersApi,
  type ListOrdersParams,
  type CreateOrderPayload,
} from "../api/ordersApi";

export const ordersQueryKeys = {
  all: ["orders"] as const,
  lists: () => [...ordersQueryKeys.all, "list"] as const,
  list: (params: ListOrdersParams) =>
    [...ordersQueryKeys.lists(), params] as const,
  details: () => [...ordersQueryKeys.all, "detail"] as const,
  detail: (id: number) => [...ordersQueryKeys.details(), id] as const,
};

export function useOrders(params: ListOrdersParams = {}) {
  return useQuery({
    queryKey: ordersQueryKeys.list(params),
    queryFn: () => ordersApi.list(params),
  });
}

export function useOrder(id: number) {
  return useQuery({
    queryKey: ordersQueryKeys.detail(id),
    queryFn: () => ordersApi.getById(id),
    enabled: id > 0,
  });
}

/**
 * Invalidates all order-related queries so list and detail views
 * refetch the latest state after a successful transition.
 */
function useInvalidateOrders() {
  const queryClient = useQueryClient();
  return (id?: number) => {
    queryClient.invalidateQueries({ queryKey: ordersQueryKeys.lists() });
    if (id !== undefined)
      queryClient.invalidateQueries({ queryKey: ordersQueryKeys.detail(id) });
  };
}

export function useCreateOrder() {
  const invalidate = useInvalidateOrders();
  return useMutation({
    mutationFn: ({
      payload,
      actingUser,
    }: {
      payload: CreateOrderPayload;
      actingUser: string;
    }) => ordersApi.create(payload, actingUser),
    onSuccess: () => invalidate(),
  });
}

export function useSubmitOrder() {
  const invalidate = useInvalidateOrders();
  return useMutation({
    mutationFn: ({ id, actingUser }: { id: number; actingUser: string }) =>
      ordersApi.submit(id, actingUser),
    onSuccess: (_data, { id }) => invalidate(id),
  });
}

export function useApproveOrder() {
  const invalidate = useInvalidateOrders();
  return useMutation({
    mutationFn: ({ id, actingUser }: { id: number; actingUser: string }) =>
      ordersApi.approve(id, actingUser),
    onSuccess: (_data, { id }) => invalidate(id),
  });
}

export function useRejectOrder() {
  const invalidate = useInvalidateOrders();
  return useMutation({
    mutationFn: ({
      id,
      actingUser,
      reason,
    }: {
      id: number;
      actingUser: string;
      reason: string;
    }) => ordersApi.reject(id, actingUser, reason),
    onSuccess: (_data, { id }) => invalidate(id),
  });
}

export function useAIRecommendation(orderId: number) {
  return useQuery({
    queryKey: [...ordersQueryKeys.detail(orderId), "ai-recommendation"],
    queryFn: () => ordersApi.getAiRecommendation(orderId),
    enabled: false, // lazy — only fires when refetch() is called
    staleTime: 5 * 60 * 1000, // cache AI result 5 min (don't re-call on remount)
    gcTime: 10 * 60 * 1000,
    retry: false, // AI failures shouldn't auto-retry (costly + slow)
  });
}
