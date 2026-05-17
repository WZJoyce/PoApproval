import { BrowserRouter, Routes, Route } from "react-router-dom";
import { OrderListPage } from "@/features/orders/pages/OrderListPage";
import { OrderDetailPage } from "@/features/orders/pages/OrderDetailPage";

function App() {
  return (
    <BrowserRouter>
      <div className="min-h-screen">
        <header className="border-b border-border bg-background">
          <div className="mx-auto max-w-6xl px-6 py-4">
            <h1 className="text-xl font-semibold">PoApproval</h1>
          </div>
        </header>
        <main className="mx-auto max-w-6xl px-6 py-8">
          <Routes>
            <Route path="/" element={<OrderListPage />} />
            <Route path="/orders/:id" element={<OrderDetailPage />} />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}

export default App;
