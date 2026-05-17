import { useQuery } from "@tanstack/react-query";
import { ordersApi, type ListOrdersParams } from "../api/ordersApi";

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
