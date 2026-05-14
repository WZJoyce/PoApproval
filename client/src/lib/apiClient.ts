import axios, { AxiosError } from "axios";
import type { ProblemDetails } from "@/features/orders/types";

const baseURL = import.meta.env.VITE_API_URL;

if (!baseURL) {
  throw new Error("VITE_API_URL is not defined. Check your .env file.");
}

export const apiClient = axios.create({
  baseURL,
  headers: {
    "Content-Type": "application/json",
  },
});

export class ApiError extends Error {
  public readonly status: number;
  public readonly problem: ProblemDetails | null;

  constructor(
    message: string,
    status: number,
    problem: ProblemDetails | null = null,
  ) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.problem = problem;
  }
}

apiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError<ProblemDetails>) => {
    const status = error.response?.status ?? 0;
    const problem = error.response?.data ?? null;
    const message =
      problem?.detail ??
      problem?.title ??
      error.message ??
      "An unexpected error occurred.";

    return Promise.reject(new ApiError(message, status, problem));
  },
);
