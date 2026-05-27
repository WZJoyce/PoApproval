import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { ApiError } from "@/lib/apiClient";
import { useActingUser } from "@/features/auth/actingUser";
import { useCreateOrder } from "../hooks/useOrders";

const createOrderSchema = z.object({
  orderNo: z
    .string()
    .trim()
    .min(3, "Order number must be at least 3 characters.")
    .max(50, "Order number must be at most 50 characters."),
  amount: z
    .number({ message: "Amount is required." })
    .positive("Amount must be greater than zero.")
    .max(1_000_000_000, "Amount is too large."),
});

type CreateOrderFormValues = z.infer<typeof createOrderSchema>;

function describeError(error: unknown): string {
  if (error instanceof ApiError) {
    if (error.status === 409) {
      return "An order with this number already exists. Use a different number.";
    }
    if (error.status === 422) {
      return error.message;
    }
    return error.message;
  }
  return "An unexpected error occurred. Please try again.";
}

export function CreateOrderDialog() {
  const [open, setOpen] = useState(false);
  const { actingUser } = useActingUser();
  const createOrder = useCreateOrder();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CreateOrderFormValues>({
    resolver: zodResolver(createOrderSchema),
    defaultValues: { orderNo: "", amount: undefined as unknown as number },
  });

  const onSubmit = (values: CreateOrderFormValues) => {
    createOrder.mutate(
      { payload: values, actingUser },
      {
        onSuccess: () => {
          toast.success(`Order ${values.orderNo} created.`);
          reset();
          setOpen(false);
        },
        onError: (error) => toast.error(describeError(error)),
      },
    );
  };

  const handleOpenChange = (nextOpen: boolean) => {
    setOpen(nextOpen);
    // Reset form state when the dialog closes so the next opening is clean.
    if (!nextOpen) {
      reset();
    }
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger asChild>
        <Button>+ New Order</Button>
      </DialogTrigger>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Create new order</DialogTitle>
          <DialogDescription>
            Drafts can be edited and resubmitted before going through approval.
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="orderNo">Order Number</Label>
            <Input
              id="orderNo"
              placeholder="e.g. PO-2026-001"
              autoComplete="off"
              {...register("orderNo")}
              aria-invalid={errors.orderNo ? "true" : "false"}
              aria-describedby={errors.orderNo ? "orderNo-error" : undefined}
            />
            {errors.orderNo && (
              <p id="orderNo-error" className="text-sm text-danger">
                {errors.orderNo.message}
              </p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="amount">Amount (USD)</Label>
            <Input
              id="amount"
              type="number"
              step="0.01"
              placeholder="0.00"
              {...register("amount", { valueAsNumber: true })}
              aria-invalid={errors.amount ? "true" : "false"}
              aria-describedby={errors.amount ? "amount-error" : undefined}
            />
            {errors.amount && (
              <p id="amount-error" className="text-sm text-danger">
                {errors.amount.message}
              </p>
            )}
          </div>

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={() => handleOpenChange(false)}
              disabled={createOrder.isPending}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={createOrder.isPending}>
              {createOrder.isPending ? "Creating…" : "Create"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
