import { CartItem, Theme } from "../App";
import { Button } from "./ui/button";
import {
  Flame,
  Snowflake,
  Wind,
  Shirt,
  Sparkles,
  Package,
} from "lucide-react";
import { toast } from "sonner";

interface ServiceGridProps {
  selectedCategory: string;
  onCategoryChange: (category: string) => void;
  onAddToCart: (item: CartItem) => void;
  language: "EN" | "RW" | "FR";
  theme: Theme;
}

const categories = [
  { id: "washing", label: { EN: "WASH", RW: "KARABA", FR: "LAVER" } },
  { id: "drying", label: { EN: "DRY", RW: "UMISHA", FR: "SÉCHER" } },
  { id: "addon", label: { EN: "ADD-ON", RW: "ONGERAHO", FR: "SUPPLÉMENT" } },
  { id: "package", label: { EN: "PACKAGE", RW: "PAKI", FR: "FORFAIT" } },
];

const services = {
  washing: [
    {
      id: "hot-water",
      name: { EN: "Hot Water", RW: "Amazi Ashyushye", FR: "Eau Chaude" },
      price: 5000,
      icon: Flame,
      color: "bg-[#FEE2E2]",
      iconColor: "text-[#EF4444]",
    },
    {
      id: "cold-water",
      name: { EN: "Cold Water", RW: "Amazi Akonje", FR: "Eau Froide" },
      price: 6000,
      icon: Snowflake,
      color: "bg-[#DBEAFE]",
      iconColor: "text-[#3B82F6]",
    },
    {
      id: "express-wash",
      name: { EN: "Express Wash", RW: "Karaba Vuba", FR: "Lavage Express" },
      price: 8000,
      icon: Sparkles,
      color: "bg-[#FEF3C7]",
      iconColor: "text-[#F59E0B]",
    },
  ],
  drying: [
    {
      id: "regular-dry",
      name: { EN: "Regular Dry", RW: "Umisha Bisanzwe", FR: "Séchage Normal" },
      price: 3000,
      icon: Wind,
      color: "bg-[#D1FAE5]",
      iconColor: "text-[#10B981]",
    },
    {
      id: "heavy-dry",
      name: {
        EN: "Heavy Duty Dry",
        RW: "Umisha Biremereye",
        FR: "Séchage Intense",
      },
      price: 5000,
      icon: Wind,
      color: "bg-[#FED7AA]",
      iconColor: "text-[#F59E0B]",
    },
  ],
  addon: [
    {
      id: "ironing",
      name: { EN: "Ironing", RW: "Gusukura", FR: "Repassage" },
      price: 1000,
      icon: Shirt,
      color: "bg-[#E0E7FF]",
      iconColor: "text-[#6366F1]",
    },
    {
      id: "bleach",
      name: { EN: "Bleach Treatment (FREE)", RW: "Kurera (Ubuntu)", FR: "Traitement Javel (GRATUIT)" },
      price: 0,
      icon: Sparkles,
      color: "bg-[#D1FAE5]",
      iconColor: "text-[#10B981]",
    },
  ],
  package: [
    {
      id: "complete-package",
      name: {
        EN: "Complete Package",
        RW: "Paki Yuzuye",
        FR: "Forfait Complet",
      },
      price: 12000,
      icon: Package,
      color: "bg-[#DCFCE7]",
      iconColor: "text-[#16A34A]",
      description: { 
        EN: "Wash + Dry + Iron + Bleach", 
        RW: "Karaba + Umisha + Gusukura + Kurera", 
        FR: "Laver + Sécher + Repasser + Javel" 
      },
    },
  ],
};

export default function ServiceGrid({
  selectedCategory,
  onCategoryChange,
  onAddToCart,
  language,
  theme,
}: ServiceGridProps) {
  const handleServiceClick = (service: any) => {
    const item: CartItem = {
      id: `${service.id}-${Date.now()}`,
      name: service.name[language],
      price: service.price,
      addons: [],
      quantity: 1,
    };

    onAddToCart(item);
    toast.success(`${service.name[language]} added to cart`);
  };

  const currentServices =
    services[selectedCategory as keyof typeof services] || [];

  return (
    <div className="flex h-full flex-col overflow-hidden">
      {/* Category Tabs */}
      <div className="mb-6 grid grid-cols-4 gap-3">
        {categories.map((category) => (
          <Button
            key={category.id}
            onClick={() => onCategoryChange(category.id)}
            variant={selectedCategory === category.id ? "default" : "outline"}
            className={`h-16 text-lg border-2 ${
              selectedCategory === category.id
                ? "bg-[#1E3A8A] text-white hover:bg-[#1E40AF]"
                : theme === "dark"
                ? "border-[#374151] bg-[#1F2937] text-white hover:bg-[#374151]"
                : theme === "gray"
                ? "border-[#D1D5DB] bg-white text-[#6B7280] hover:bg-[#F3F4F6] hover:text-[#1E3A8A]"
                : "border-[#E5E7EB] bg-white text-[#6B7280] hover:bg-[#F9FAFB] hover:text-[#1E3A8A]"
            }`}
          >
            {category.label[language]}
          </Button>
        ))}
      </div>

      {/* Service Grid */}
      <div className="flex-1 overflow-hidden">
        <div className="grid grid-cols-3 gap-6">
          {currentServices.map((service) => {
            const Icon = service.icon;

            return (
              <button
                key={service.id}
                onClick={() => handleServiceClick(service)}
                className={`flex h-[280px] flex-col rounded-2xl border-2 p-6 shadow-lg transition-all hover:scale-105 hover:shadow-2xl active:scale-95 ${
                  theme === "dark"
                    ? "border-[#374151] bg-[#1F2937]"
                    : theme === "gray"
                    ? "border-[#D1D5DB] bg-white"
                    : "border-[#E5E7EB] bg-white"
                }`}
              >
                {/* Icon */}
                <div className={`mx-auto mb-4 rounded-full ${service.color} p-6`}>
                  <Icon className={`h-16 w-16 ${service.iconColor}`} />
                </div>

                {/* Service Name */}
                <h3 className={`mb-2 text-center ${theme === "dark" ? "text-white" : "text-[#111827]"}`}>
                  {service.name[language]}
                </h3>

                {/* Description (for packages) */}
                {service.description && (
                  <p className={`mb-3 text-center text-sm ${theme === "dark" ? "text-[#9CA3AF]" : "text-[#6B7280]"}`}>
                    {service.description[language]}
                  </p>
                )}

                {/* Price */}
                <div className="mt-auto">
                  {service.price === 0 ? (
                    <div className="rounded-lg bg-[#D1FAE5] px-4 py-3 text-center">
                      <p className="text-2xl font-bold text-[#10B981]">FREE</p>
                      <p className="text-sm text-[#059669]">
                        {language === "EN" && "Complimentary"}
                        {language === "RW" && "Ubuntu"}
                        {language === "FR" && "Gratuit"}
                      </p>
                    </div>
                  ) : (
                    <div className="rounded-lg bg-gradient-to-r from-[#F59E0B] to-[#D97706] px-4 py-3 text-center">
                      <p className="text-3xl font-bold text-white">
                        {service.price.toLocaleString()}
                      </p>
                      <p className="text-sm text-white/90">RWF</p>
                    </div>
                  )}
                </div>

                {/* Tap to add indicator */}
                <div className={`mt-3 text-center text-sm ${theme === "dark" ? "text-[#9CA3AF]" : "text-[#6B7280]"}`}>
                  {language === "EN" && "Tap to add"}
                  {language === "RW" && "Kanda kongeraho"}
                  {language === "FR" && "Appuyez pour ajouter"}
                </div>
              </button>
            );
          })}
        </div>

        {/* Empty State */}
        {currentServices.length === 0 && (
          <div className="flex h-full items-center justify-center">
            <p className={`${theme === "dark" ? "text-[#9CA3AF]" : "text-[#6B7280]"}`}>
              No services available in this category
            </p>
          </div>
        )}
      </div>
    </div>
  );
}
