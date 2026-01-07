import { useState } from "react";
import { Toaster } from "./components/ui/sonner";
import Dashboard from "./components/Dashboard";

export type Screen = "dashboard";

export interface CartItem {
  id: string;
  name: string;
  price: number;
  addons: { name: string; price: number }[];
  quantity: number;
}

export interface Transaction {
  id: string;
  timestamp: string;
  amount: number;
  paymentMethod: "Cash" | "MoMo" | "Card";
  status: "completed" | "failed" | "pending";
  items: CartItem[];
  customer?: string;
}

export type Theme = "light" | "gray" | "dark";

function App() {
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [language, setLanguage] = useState<"EN" | "RW" | "FR">("EN");
  const [theme, setTheme] = useState<Theme>("light");

  const handleAddTransaction = (transaction: Transaction) => {
    setTransactions((prev) => [transaction, ...prev]);
  };

  return (
    <div className={`h-screen w-screen overflow-hidden ${theme === "dark" ? "dark bg-[#111827]" : theme === "gray" ? "bg-[#E5E7EB]" : "bg-[#F9FAFB]"}`}>
      <Dashboard
        onAddTransaction={handleAddTransaction}
        language={language}
        onLanguageChange={setLanguage}
        theme={theme}
        onThemeChange={setTheme}
      />

      <Toaster />
    </div>
  );
}

export default App;