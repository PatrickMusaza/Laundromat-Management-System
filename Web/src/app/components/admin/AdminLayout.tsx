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
  Sun,
  Cloud,
  Moon,
} from "lucide-react";
import { Button } from "@/app/components/ui/button";
import { Theme } from "@/app/App";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/app/components/ui/popover";

interface AdminLayoutProps {
  children: React.ReactNode;
  currentPage: string;
  onPageChange: (page: string) => void;
  onLogout: () => void;
  theme: Theme;
  onThemeChange: (theme: Theme) => void;
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
  theme,
  onThemeChange,
}: AdminLayoutProps) {
  const [sidebarOpen, setSidebarOpen] = useState(true);
  const [themeOpen, setThemeOpen] = useState(false);

  const getThemeIcon = (themeName: Theme) => {
    switch (themeName) {
      case "light":
        return Sun;
      case "gray":
        return Cloud;
      case "dark":
        return Moon;
    }
  };

  const CurrentThemeIcon = getThemeIcon(theme);

  return (
    <div className="flex h-screen overflow-hidden">
      {/* Sidebar */}
      <aside
        className={`${
          theme === "dark" ? "bg-[#1F2937] text-white" : "bg-[#1E3A8A] text-white"
        } transition-all duration-300 ${
          sidebarOpen ? "w-64" : "w-20"
        }`}
      >
        <div className="flex h-full flex-col">
          {/* Sidebar Header */}
          <div
            className={`flex items-center justify-between border-b p-4 ${
              theme === "dark" ? "border-gray-700" : "border-blue-800"
            }`}
          >
            {sidebarOpen && (
              <h1 className="font-bold">Laundromat Admin</h1>
            )}
            <Button
              variant="ghost"
              size="icon"
              className={`text-white ${
                theme === "dark" ? "hover:bg-gray-700" : "hover:bg-blue-800"
              }`}
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
                      : theme === "dark"
                      ? "text-blue-100 hover:bg-gray-700"
                      : "text-blue-100 hover:bg-blue-800"
                  }`}
                >
                  <Icon className="h-5 w-5 flex-shrink-0" />
                  {sidebarOpen && <span>{item.label}</span>}
                </button>
              );
            })}
          </nav>

          {/* Theme Switcher & Logout */}
          <div
            className={`border-t p-4 ${
              theme === "dark" ? "border-gray-700" : "border-blue-800"
            }`}
          >
            {/* Theme Switcher */}
            <Popover open={themeOpen} onOpenChange={setThemeOpen}>
              <PopoverTrigger asChild>
                <button
                  className={`mb-2 flex w-full items-center gap-3 rounded-lg px-4 py-3 text-blue-100 transition-colors ${
                    theme === "dark" ? "hover:bg-gray-700" : "hover:bg-blue-800"
                  }`}
                >
                  <CurrentThemeIcon className="h-5 w-5 flex-shrink-0" />
                  {sidebarOpen && <span>Theme</span>}
                </button>
              </PopoverTrigger>
              <PopoverContent
                className={`w-48 ${
                  theme === "dark"
                    ? "border-gray-700 bg-[#1F2937]"
                    : theme === "gray"
                    ? "bg-[#F3F4F6]"
                    : "bg-white"
                }`}
              >
                <div className="space-y-2">
                  {(["light", "gray", "dark"] as const).map((themeName) => {
                    const ThemeIcon = getThemeIcon(themeName);
                    return (
                      <button
                        key={themeName}
                        onClick={() => {
                          onThemeChange(themeName);
                          setThemeOpen(false);
                        }}
                        className={`flex w-full items-center gap-3 rounded-lg px-4 py-3 transition-colors ${
                          theme === themeName
                            ? "bg-[#1E3A8A] text-white"
                            : theme === "dark"
                            ? "text-white hover:bg-gray-700"
                            : "text-[#111827] hover:bg-[#F9FAFB]"
                        }`}
                      >
                        <ThemeIcon className="h-5 w-5" />
                        <span className="capitalize">{themeName}</span>
                      </button>
                    );
                  })}
                </div>
              </PopoverContent>
            </Popover>

            {/* Logout Button */}
            <button
              onClick={onLogout}
              className={`flex w-full items-center gap-3 rounded-lg px-4 py-3 text-blue-100 transition-colors hover:bg-red-600`}
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