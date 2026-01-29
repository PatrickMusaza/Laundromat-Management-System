import { useState } from "react";
import {
  LayoutDashboard,
  Package,
  FileText,
  Users,
  History,
  LogOut,
  Menu,
  X,
} from "lucide-react";
import { Button } from "@/app/components/ui/button";

interface AdminLayoutProps {
  children: React.ReactNode;
  currentPage: string;
  onPageChange: (page: string) => void;
  onLogout: () => void;
}

const menuItems = [
  { id: "dashboard", label: "Dashboard", icon: LayoutDashboard },
  { id: "services", label: "Services", icon: Package },
  { id: "reporting", label: "Reporting", icon: FileText },
  { id: "users", label: "Users", icon: Users },
  { id: "sales-history", label: "View Sales History", icon: History },
];

export default function AdminLayout({
  children,
  currentPage,
  onPageChange,
  onLogout,
}: AdminLayoutProps) {
  const [sidebarOpen, setSidebarOpen] = useState(true);

  return (
    <div className="flex h-screen overflow-hidden bg-gray-100">
      {/* Sidebar */}
      <aside
        className={`bg-[#1E3A8A] text-white transition-all duration-300 ${
          sidebarOpen ? "w-64" : "w-20"
        }`}
      >
        <div className="flex h-full flex-col">
          {/* Sidebar Header */}
          <div className="flex items-center justify-between border-b border-blue-800 p-4">
            {sidebarOpen && (
              <h1 className="font-bold">Laundromat Admin</h1>
            )}
            <Button
              variant="ghost"
              size="icon"
              className="text-white hover:bg-blue-800"
              onClick={() => setSidebarOpen(!sidebarOpen)}
            >
              {sidebarOpen ? <X className="h-5 w-5" /> : <Menu className="h-5 w-5" />}
            </Button>
          </div>

          {/* Menu Items */}
          <nav className="flex-1 space-y-1 p-4">
            {menuItems.map((item) => {
              const Icon = item.icon;
              const isActive = currentPage === item.id;
              return (
                <button
                  key={item.id}
                  onClick={() => onPageChange(item.id)}
                  className={`flex w-full items-center gap-3 rounded-lg px-4 py-3 transition-colors ${
                    isActive
                      ? "bg-[#F59E0B] text-white"
                      : "text-blue-100 hover:bg-blue-800"
                  }`}
                >
                  <Icon className="h-5 w-5 flex-shrink-0" />
                  {sidebarOpen && <span>{item.label}</span>}
                </button>
              );
            })}
          </nav>

          {/* Logout Button */}
          <div className="border-t border-blue-800 p-4">
            <button
              onClick={onLogout}
              className="flex w-full items-center gap-3 rounded-lg px-4 py-3 text-blue-100 transition-colors hover:bg-red-600"
            >
              <LogOut className="h-5 w-5 flex-shrink-0" />
              {sidebarOpen && <span>Logout</span>}
            </button>
          </div>
        </div>
      </aside>

      {/* Main Content */}
      <main className="flex-1 overflow-y-auto">
        <div className="p-6">{children}</div>
      </main>
    </div>
  );
}
