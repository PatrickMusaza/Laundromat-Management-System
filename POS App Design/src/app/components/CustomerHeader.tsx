import { useState } from "react";
import { Theme } from "../App";
import { Languages, Palette, Sun, Cloud, Moon } from "lucide-react";
import { Button } from "./ui/button";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "./ui/popover";

interface CustomerHeaderProps {
  language: "EN" | "RW" | "FR";
  onLanguageChange: (lang: "EN" | "RW" | "FR") => void;
  theme: Theme;
  onThemeChange: (theme: Theme) => void;
}

const translations = {
  EN: {
    welcome: "Welcome to Laundromat",
    selectService: "Select Your Services",
    language: "Language",
    theme: "Appearance",
    light: "Light",
    gray: "Gray",
    dark: "Dark",
  },
  RW: {
    welcome: "Murakaza Neza",
    selectService: "Hitamo Serivisi Zawe",
    language: "Ururimi",
    theme: "Isura",
    light: "Yera",
    gray: "Icyatsi",
    dark: "Umwijima",
  },
  FR: {
    welcome: "Bienvenue à la Laverie",
    selectService: "Sélectionnez Vos Services",
    language: "Langue",
    theme: "Apparence",
    light: "Clair",
    gray: "Gris",
    dark: "Sombre",
  },
};

export default function CustomerHeader({
  language,
  onLanguageChange,
  theme,
  onThemeChange,
}: CustomerHeaderProps) {
  const [languageOpen, setLanguageOpen] = useState(false);
  const [themeOpen, setThemeOpen] = useState(false);

  const t = translations[language];

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
    <div className={`border-b-2 ${theme === "dark" ? "border-[#374151] bg-[#1F2937]" : theme === "gray" ? "border-[#D1D5DB] bg-white" : "border-[#E5E7EB] bg-white"}`}>
      <div className="flex items-center justify-between px-8 py-6">
        {/* Logo and Title */}
        <div>
          <h1 className={`mb-1 ${theme === "dark" ? "text-white" : "text-[#1E3A8A]"}`}>
            {t.welcome}
          </h1>
          <p className={`${theme === "dark" ? "text-[#9CA3AF]" : "text-[#6B7280]"}`}>
            {t.selectService}
          </p>
        </div>

        {/* Controls */}
        <div className="flex items-center gap-4">
          {/* Language Selector */}
          <Popover open={languageOpen} onOpenChange={setLanguageOpen}>
            <PopoverTrigger asChild>
              <Button
                variant="outline"
                className={`h-14 gap-3 border-2 px-6 ${
                  theme === "dark"
                    ? "border-[#374151] bg-[#374151] text-white hover:bg-[#4B5563]"
                    : theme === "gray"
                    ? "border-[#D1D5DB] bg-[#F3F4F6] hover:bg-[#E5E7EB]"
                    : "border-[#E5E7EB] bg-white hover:bg-[#F9FAFB]"
                }`}
              >
                <Languages className="h-6 w-6" />
                <div className="flex flex-col items-start">
                  <span className="text-xs">{t.language}</span>
                  <span className="font-semibold">{language}</span>
                </div>
              </Button>
            </PopoverTrigger>
            <PopoverContent className={`w-48 ${theme === "dark" ? "border-[#374151] bg-[#1F2937]" : theme === "gray" ? "bg-[#F3F4F6]" : "bg-white"}`}>
              <div className="space-y-2">
                {(["EN", "RW", "FR"] as const).map((lang) => (
                  <button
                    key={lang}
                    onClick={() => {
                      onLanguageChange(lang);
                      setLanguageOpen(false);
                    }}
                    className={`w-full rounded-lg px-4 py-3 text-left transition-colors ${
                      language === lang
                        ? theme === "dark"
                          ? "bg-[#1E3A8A] text-white"
                          : "bg-[#1E3A8A] text-white"
                        : theme === "dark"
                        ? "text-white hover:bg-[#374151]"
                        : "text-[#111827] hover:bg-[#F9FAFB]"
                    }`}
                  >
                    {lang === "EN" && "English"}
                    {lang === "RW" && "Kinyarwanda"}
                    {lang === "FR" && "Français"}
                  </button>
                ))}
              </div>
            </PopoverContent>
          </Popover>

          {/* Theme Selector */}
          <Popover open={themeOpen} onOpenChange={setThemeOpen}>
            <PopoverTrigger asChild>
              <Button
                variant="outline"
                className={`h-14 gap-3 border-2 px-6 ${
                  theme === "dark"
                    ? "border-[#374151] bg-[#374151] text-white hover:bg-[#4B5563]"
                    : theme === "gray"
                    ? "border-[#D1D5DB] bg-[#F3F4F6] hover:bg-[#E5E7EB]"
                    : "border-[#E5E7EB] bg-white hover:bg-[#F9FAFB]"
                }`}
              >
                <CurrentThemeIcon className="h-6 w-6" />
                <div className="flex flex-col items-start">
                  <span className="text-xs">{t.theme}</span>
                  <span className="font-semibold capitalize">{t[theme]}</span>
                </div>
              </Button>
            </PopoverTrigger>
            <PopoverContent className={`w-48 ${theme === "dark" ? "border-[#374151] bg-[#1F2937]" : theme === "gray" ? "bg-[#F3F4F6]" : "bg-white"}`}>
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
                          ? "text-white hover:bg-[#374151]"
                          : "text-[#111827] hover:bg-[#F9FAFB]"
                      }`}
                    >
                      <ThemeIcon className="h-5 w-5" />
                      <span className="capitalize">{t[themeName]}</span>
                    </button>
                  );
                })}
              </div>
            </PopoverContent>
          </Popover>
        </div>
      </div>
    </div>
  );
}
