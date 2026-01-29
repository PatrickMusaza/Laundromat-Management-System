import { Screen } from "../App";
import {
  LayoutDashboard,
  FileText,
  History,
  BarChart3,
  Settings,
  LogOut,
} from "lucide-react";
import { Button } from "./ui/button";

interface FooterProps {
  currentScreen: Screen;
  onNavigate: (screen: Screen) => void;
  onLogout: () => void;
}

export default function Footer({
  currentScreen,
  onNavigate,
  onLogout,
}: FooterProps) {
  const navItems = [
    { id: "dashboard" as Screen, label: "Dashboard", icon: LayoutDashboard },
    { id: "dashboard" as Screen, label: "New Trans", icon: FileText },
    { id: "history" as Screen, label: "History", icon: History },
    { id: "dashboard" as Screen, label: "Reports", icon: BarChart3 },
    { id: "settings" as Screen, label: "Settings", icon: Settings },
  ];

  return (
    <div className="border-t-2 border-[#E5E7EB] bg-white px-6 py-3">
      <div className="flex items-center justify-between">
        <div className="flex gap-2">
          {navItems.map((item) => {
            const Icon = item.icon;
            const isActive = currentScreen === item.id;
            return (
              <Button
                key={item.label}
                onClick={() => onNavigate(item.id)}
                variant={isActive ? "default" : "ghost"}
                className={`h-12 min-w-[120px] gap-2 ${
                  isActive
                    ? "bg-[#1E3A8A] text-white hover:bg-[#1E40AF]"
                    : "text-[#6B7280] hover:bg-[#F9FAFB] hover:text-[#1E3A8A]"
                }`}
              >
                <Icon className="h-5 w-5" />
                {item.label}
              </Button>
            );
          })}
        </div>

        <Button
          onClick={onLogout}
          variant="ghost"
          className="h-12 gap-2 text-[#EF4444] hover:bg-[#FEE2E2] hover:text-[#EF4444]"
        >
          <LogOut className="h-5 w-5" />
          Logout
        </Button>
      </div>
    </div>
  );
}
