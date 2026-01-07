import { useState } from "react";
import { Button } from "./ui/button";
import { Input } from "./ui/input";
import { Label } from "./ui/label";
import { Checkbox } from "./ui/checkbox";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "./ui/select";
import { WashingMachine, Wifi, WifiOff } from "lucide-react";
import { toast } from "sonner";

interface LoginScreenProps {
  onLogin: (username: string, branch: string) => void;
  language: "EN" | "RW" | "FR";
  onLanguageChange: (lang: "EN" | "RW" | "FR") => void;
}

const translations = {
  EN: {
    title: "LAUNDROMAT POS SYSTEM",
    username: "Username",
    password: "Password",
    rememberMe: "Remember me",
    forgotPassword: "Forgot Password?",
    login: "LOGIN",
    branch: "Select Branch",
    online: "Online",
    offline: "Offline",
  },
  RW: {
    title: "SISITEMU YA POS Y'IMESA",
    username: "Izina ry'ukoresha",
    password: "Ijambo ry'ibanga",
    rememberMe: "Nzibuke",
    forgotPassword: "Wibagiwe ijambo ry'ibanga?",
    login: "INJIRA",
    branch: "Hitamo ishami",
    online: "Kumurongo",
    offline: "Ntabwo uri kumurongo",
  },
  FR: {
    title: "SYSTÈME POS DE LAVERIE",
    username: "Nom d'utilisateur",
    password: "Mot de passe",
    rememberMe: "Se souvenir de moi",
    forgotPassword: "Mot de passe oublié?",
    login: "CONNEXION",
    branch: "Sélectionner la succursale",
    online: "En ligne",
    offline: "Hors ligne",
  },
};

export default function LoginScreen({
  onLogin,
  language,
  onLanguageChange,
}: LoginScreenProps) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [branch, setBranch] = useState("");
  const [rememberMe, setRememberMe] = useState(false);
  const [isOnline] = useState(true);

  const t = translations[language];

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!username || !password || !branch) {
      toast.error("Please fill in all fields");
      return;
    }
    onLogin(username, branch);
  };

  return (
    <div className="flex h-full w-full items-center justify-center bg-gradient-to-br from-[#1E3A8A] to-[#1E40AF]">
      <div className="w-full max-w-md rounded-xl bg-white p-12 shadow-2xl">
        {/* Logo and Title */}
        <div className="mb-8 text-center">
          <div className="mb-4 flex justify-center">
            <div className="rounded-full bg-[#1E3A8A] p-6">
              <WashingMachine className="h-16 w-16 text-white" />
            </div>
          </div>
          <h1 className="mb-2 text-[#1E3A8A]">{t.title}</h1>
          <div className="flex items-center justify-center gap-2">
            {isOnline ? (
              <>
                <Wifi className="h-4 w-4 text-[#10B981]" />
                <span className="text-[#10B981]">{t.online}</span>
              </>
            ) : (
              <>
                <WifiOff className="h-4 w-4 text-[#EF4444]" />
                <span className="text-[#EF4444]">{t.offline}</span>
              </>
            )}
          </div>
        </div>

        {/* Login Form */}
        <form onSubmit={handleSubmit} className="space-y-6">
          {/* Branch Selection */}
          <div className="space-y-2">
            <Label htmlFor="branch">{t.branch}</Label>
            <Select value={branch} onValueChange={setBranch}>
              <SelectTrigger className="h-12 border-2">
                <SelectValue placeholder={t.branch} />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="kigali-city">Kigali City Center</SelectItem>
                <SelectItem value="kimironko">Kimironko Branch</SelectItem>
                <SelectItem value="remera">Remera Branch</SelectItem>
                <SelectItem value="nyabugogo">Nyabugogo Branch</SelectItem>
              </SelectContent>
            </Select>
          </div>

          {/* Username */}
          <div className="space-y-2">
            <Label htmlFor="username">{t.username}</Label>
            <Input
              id="username"
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              className="h-12 border-2"
              placeholder={t.username}
            />
          </div>

          {/* Password */}
          <div className="space-y-2">
            <Label htmlFor="password">{t.password}</Label>
            <Input
              id="password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              className="h-12 border-2"
              placeholder="••••••••"
            />
          </div>

          {/* Remember Me and Forgot Password */}
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-2">
              <Checkbox
                id="remember"
                checked={rememberMe}
                onCheckedChange={(checked) =>
                  setRememberMe(checked as boolean)
                }
              />
              <label
                htmlFor="remember"
                className="cursor-pointer select-none text-[#6B7280]"
              >
                {t.rememberMe}
              </label>
            </div>
            <button
              type="button"
              className="text-[#1E3A8A] hover:underline"
              onClick={() => toast.info("Contact administrator")}
            >
              {t.forgotPassword}
            </button>
          </div>

          {/* Login Button */}
          <Button
            type="submit"
            className="h-14 w-full bg-[#1E3A8A] hover:bg-[#1E40AF]"
          >
            {t.login}
          </Button>

          {/* Language Switcher */}
          <div className="flex items-center justify-center gap-4 pt-4">
            <button
              type="button"
              onClick={() => onLanguageChange("EN")}
              className={`px-3 py-1 ${
                language === "EN"
                  ? "border-b-2 border-[#F59E0B] text-[#F59E0B]"
                  : "text-[#6B7280]"
              }`}
            >
              EN
            </button>
            <span className="text-[#E5E7EB]">|</span>
            <button
              type="button"
              onClick={() => onLanguageChange("RW")}
              className={`px-3 py-1 ${
                language === "RW"
                  ? "border-b-2 border-[#F59E0B] text-[#F59E0B]"
                  : "text-[#6B7280]"
              }`}
            >
              RW
            </button>
            <span className="text-[#E5E7EB]">|</span>
            <button
              type="button"
              onClick={() => onLanguageChange("FR")}
              className={`px-3 py-1 ${
                language === "FR"
                  ? "border-b-2 border-[#F59E0B] text-[#F59E0B]"
                  : "text-[#6B7280]"
              }`}
            >
              FR
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}
