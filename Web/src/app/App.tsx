import { useState } from "react";
import { Toaster } from "./components/ui/sonner";
import Dashboard from "./components/Dashboard";
import LoginModal from "./components/admin/LoginModal";
import AdminLayout from "./components/admin/AdminLayout";
import AdminDashboard from "./components/admin/pages/AdminDashboard";
import ServicesPage from "./components/admin/pages/ServicesPage";
import ReportingPage from "./components/admin/pages/ReportingPage";
import UsersPage from "./components/admin/pages/UsersPage";
import SalesHistoryPage from "./components/admin/pages/SalesHistoryPage";

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
  const [showLoginModal, setShowLoginModal] = useState(false);
  const [isAdmin, setIsAdmin] = useState(false);
  const [adminPage, setAdminPage] = useState("dashboard");

  const handleAddTransaction = (transaction: Transaction) => {
    setTransactions((prev) => [transaction, ...prev]);
  };

  const handleLogin = (username: string, password: string) => {
    // Mock authentication
    if (username === "admin" && password === "admin") {
      setIsAdmin(true);
    }
  };

  const handleLogout = () => {
    setIsAdmin(false);
    setAdminPage("dashboard");
  };

  if (isAdmin) {
    return (
      <div className={`h-screen w-screen overflow-hidden ${theme === "dark" ? "bg-[#111827]" : theme === "gray" ? "bg-[#E5E7EB]" : "bg-gray-100"}`}>
        <AdminLayout
          currentPage={adminPage}
          onPageChange={setAdminPage}
          onLogout={handleLogout}
          theme={theme}
          onThemeChange={setTheme}
        >
          {adminPage === "dashboard" && <AdminDashboard theme={theme} />}
          {adminPage === "services" && <ServicesPage theme={theme} />}
          {adminPage === "reporting" && <ReportingPage theme={theme} />}
          {adminPage === "users" && <UsersPage theme={theme} />}
          {adminPage === "sales-history" && <SalesHistoryPage theme={theme} />}
        </AdminLayout>
        <Toaster />
      </div>
    );
  }

  return (
    <div className={`h-screen w-screen overflow-hidden ${theme === "dark" ? "dark bg-[#111827]" : theme === "gray" ? "bg-[#E5E7EB]" : "bg-[#F9FAFB]"}`}>
      <Dashboard
        onAddTransaction={handleAddTransaction}
        language={language}
        onLanguageChange={setLanguage}
        theme={theme}
        onThemeChange={setTheme}
        onLoginClick={() => setShowLoginModal(true)}
      />

      <LoginModal
        open={showLoginModal}
        onClose={() => setShowLoginModal(false)}
        onLogin={handleLogin}
      />

      <Toaster />
    </div>
  );
}

export default App;