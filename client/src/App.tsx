import { BrowserRouter, Routes, Route, Link } from "react-router-dom";
import { Toaster } from "@/components/ui/sonner";
import { OrderListPage } from "@/features/orders/pages/OrderListPage";
import { OrderDetailPage } from "@/features/orders/pages/OrderDetailPage";

function App() {
  return (
    <BrowserRouter>
      <div className="min-h-screen">
        <header className="border-b border-border bg-background">
          <div className="mx-auto flex max-w-6xl items-center justify-between px-6 py-4">
            <Link to="/" className="flex items-center gap-2.5">
              <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary font-bold text-primary-foreground">
                P
              </div>
              <div className="flex flex-col leading-none">
                <span className="text-base font-semibold">PoApproval</span>
                <span className="text-xs text-muted-foreground">
                  Purchase Order Approval
                </span>
              </div>
            </Link>
          </div>
        </header>
        <main className="mx-auto max-w-6xl px-6 py-8">
          <Routes>
            <Route path="/" element={<OrderListPage />} />
            <Route path="/orders/:id" element={<OrderDetailPage />} />
          </Routes>
        </main>
      </div>
      <Toaster richColors position="top-right" />
    </BrowserRouter>
  );
}

export default App;
