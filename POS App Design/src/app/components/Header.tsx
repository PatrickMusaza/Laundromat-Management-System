import { User } from "../App";
import { Wifi, CheckCircle, Clock } from "lucide-react";
import { useEffect, useState } from "react";

interface HeaderProps {
  user: User;
}

export default function Header({ user }: HeaderProps) {
  const [currentTime, setCurrentTime] = useState(new Date());

  useEffect(() => {
    const timer = setInterval(() => {
      setCurrentTime(new Date());
    }, 1000);
    return () => clearInterval(timer);
  }, []);

  const formatTime = (date: Date) => {
    return date.toLocaleTimeString("en-US", {
      hour: "2-digit",
      minute: "2-digit",
      hour12: false,
    });
  };

  return (
    <div className="border-b-2 border-[#E5E7EB] bg-white">
      {/* Top Bar */}
      <div className="flex items-center justify-between px-6 py-3">
        <div className="flex items-center gap-6">
          <div className="flex items-center gap-2">
            <span className="text-[#6B7280]">Branch:</span>
            <span className="text-[#111827]">{user.branch}</span>
          </div>
          <div className="h-6 w-px bg-[#E5E7EB]" />
          <div className="flex items-center gap-2">
            <span className="text-[#6B7280]">User:</span>
            <span className="text-[#111827]">
              {user.username} ({user.role})
            </span>
          </div>
          <div className="h-6 w-px bg-[#E5E7EB]" />
          <div className="flex items-center gap-2">
            <Clock className="h-4 w-4 text-[#6B7280]" />
            <span className="text-[#111827]">{formatTime(currentTime)}</span>
          </div>
        </div>

        <div className="flex items-center gap-6">
          <div className="flex items-center gap-2">
            <span className="text-[#6B7280]">Status:</span>
            <div className="flex items-center gap-1">
              <div className="h-2 w-2 rounded-full bg-[#10B981]" />
              <span className="text-[#10B981]">Online</span>
            </div>
          </div>
          <div className="h-6 w-px bg-[#E5E7EB]" />
          <div className="flex items-center gap-2">
            <span className="text-[#6B7280]">RRA:</span>
            <div className="flex items-center gap-1">
              <Wifi className="h-4 w-4 text-[#10B981]" />
              <span className="text-[#10B981]">Connected</span>
            </div>
          </div>
          <div className="h-6 w-px bg-[#E5E7EB]" />
          <div className="flex items-center gap-2">
            <span className="text-[#6B7280]">Sync:</span>
            <div className="flex items-center gap-1">
              <CheckCircle className="h-4 w-4 text-[#10B981]" />
              <span className="text-[#10B981]">Up to date</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
