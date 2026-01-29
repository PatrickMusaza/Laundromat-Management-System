import { CartItem, Theme } from "../App";
import { Button } from "./ui/button";
import { Trash2, ShoppingCart as CartIcon } from "lucide-react";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "./ui/select";

interface ShoppingCartProps {
  cart: CartItem[];
  onRemoveItem: (itemId: string) => void;
  onUpdateQuantity: (itemId: string, quantity: number) => void;
  onProcessPayment: () => void;
  subtotal: number;
  tax: number;
  total: number;
  language: "EN" | "RW" | "FR";
  theme: Theme;
}

const translations = {
  EN: {
    cart: "Shopping Cart",
    empty: "Your cart is empty",
    emptyMsg: "Add services to get started",
    quantity: "Qty",
    subtotal: "Subtotal",
    tax: "Tax (10%)",
    total: "TOTAL",
    payment: "Payment Methods",
    process: "PROCESS PAYMENT",
    cash: "CASH",
    momo: "MOMO",
    card: "CARD",
  },
  RW: {
    cart: "Igitebo",
    empty: "Igitebo cyawe kirimo ubusa",
    emptyMsg: "Ongeraho serivisi",
    quantity: "Umubare",
    subtotal: "Igiteranyo",
    tax: "Umusoro (10%)",
    total: "IGITERANYO",
    payment: "Uburyo bwo kwishyura",
    process: "KWISHYURA",
    cash: "AMAFARANGA",
    momo: "MOMO",
    card: "KARITA",
  },
  FR: {
    cart: "Panier",
    empty: "Votre panier est vide",
    emptyMsg: "Ajoutez des services pour commencer",
    quantity: "Qté",
    subtotal: "Sous-total",
    tax: "Taxe (10%)",
    total: "TOTAL",
    payment: "Méthodes de paiement",
    process: "TRAITER LE PAIEMENT",
    cash: "ESPÈCES",
    momo: "MOMO",
    card: "CARTE",
  },
};

export default function ShoppingCart({
  cart,
  onRemoveItem,
  onUpdateQuantity,
  onProcessPayment,
  subtotal,
  tax,
  total,
  language,
  theme,
}: ShoppingCartProps) {
  const t = translations[language];

  return (
    <div
      className={`flex h-full flex-col rounded-xl border-2 p-4 shadow-lg ${
        theme === "dark"
          ? "border-[#374151] bg-[#1F2937]"
          : theme === "gray"
          ? "border-[#D1D5DB] bg-white"
          : "border-[#E5E7EB] bg-white"
      }`}
    >
      {/* Header */}
      <div
        className={`mb-4 flex items-center gap-2 border-b-2 pb-3 ${
          theme === "dark" ? "border-[#374151]" : "border-[#E5E7EB]"
        }`}
      >
        <CartIcon
          className={`h-6 w-6 ${
            theme === "dark" ? "text-white" : "text-[#1E3A8A]"
          }`}
        />
        <h2 className={theme === "dark" ? "text-white" : "text-[#1E3A8A]"}>{t.cart}</h2>
        {cart.length > 0 && (
          <span className="ml-auto flex h-6 w-6 items-center justify-center rounded-full bg-[#F59E0B] text-white">
            {cart.length}
          </span>
        )}
      </div>

      {/* Cart Items */}
      <div className="mb-4 flex-1 space-y-2 overflow-y-auto">
        {cart.length === 0 ? (
          <div className="flex h-full flex-col items-center justify-center text-center">
            <CartIcon
              className={`mb-3 h-16 w-16 ${
                theme === "dark" ? "text-[#4B5563]" : "text-[#E5E7EB]"
              }`}
            />
            <p
              className={`mb-1 ${
                theme === "dark" ? "text-[#9CA3AF]" : "text-[#6B7280]"
              }`}
            >
              {t.empty}
            </p>
            <p className={theme === "dark" ? "text-[#6B7280]" : "text-[#9CA3AF]"}>{t.emptyMsg}</p>
          </div>
        ) : (
          cart.map((item) => {
            const itemPrice =
              item.price +
              item.addons.reduce((sum, addon) => sum + addon.price, 0);
            const itemTotal = itemPrice * item.quantity;

            return (
              <div
                key={item.id}
                className={`rounded-lg border p-3 ${
                  theme === "dark"
                    ? "border-[#374151] bg-[#111827]"
                    : theme === "gray"
                    ? "border-[#D1D5DB] bg-[#F3F4F6]"
                    : "border-[#E5E7EB] bg-[#F9FAFB]"
                }`}
              >
                <div className="mb-2 flex items-start justify-between">
                  <div className="flex-1">
                    <p
                      className={theme === "dark" ? "text-white" : "text-[#111827]"}
                    >
                      {item.name}
                    </p>
                    <p className="text-[#F59E0B]">
                      {item.price.toLocaleString()} RWF
                    </p>
                  </div>
                  <button
                    onClick={() => onRemoveItem(item.id)}
                    className="text-[#EF4444] hover:text-[#DC2626]"
                  >
                    <Trash2 className="h-5 w-5" />
                  </button>
                </div>

                {item.addons.length > 0 && (
                  <div className="mb-2 space-y-1 border-l-2 border-[#F59E0B] pl-2">
                    {item.addons.map((addon, idx) => (
                      <p
                        key={idx}
                        className={
                          theme === "dark" ? "text-[#9CA3AF]" : "text-[#6B7280]"
                        }
                      >
                        + {addon.name}: {addon.price.toLocaleString()} RWF
                      </p>
                    ))}
                  </div>
                )}

                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <span
                      className={
                        theme === "dark" ? "text-[#9CA3AF]" : "text-[#6B7280]"
                      }
                    >
                      {t.quantity}:
                    </span>
                    <Select
                      value={item.quantity.toString()}
                      onValueChange={(value) =>
                        onUpdateQuantity(item.id, parseInt(value))
                      }
                    >
                      <SelectTrigger className="h-8 w-16">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        {[1, 2, 3, 4, 5, 6, 7, 8, 9, 10].map((num) => (
                          <SelectItem key={num} value={num.toString()}>
                            {num}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                  <p
                    className={
                      theme === "dark" ? "text-white" : "text-[#111827]"
                    }
                  >
                    {itemTotal.toLocaleString()} RWF
                  </p>
                </div>
              </div>
            );
          })
        )}
      </div>

      {/* Totals */}
      {cart.length > 0 && (
        <>
          <div
            className={`mb-4 space-y-2 rounded-lg border-2 p-4 ${
              theme === "dark"
                ? "border-[#374151] bg-[#111827]"
                : theme === "gray"
                ? "border-[#D1D5DB] bg-[#F3F4F6]"
                : "border-[#E5E7EB] bg-[#F9FAFB]"
            }`}
          >
            <div
              className={`flex justify-between ${
                theme === "dark" ? "text-[#9CA3AF]" : "text-[#6B7280]"
              }`}
            >
              <span>{t.subtotal}:</span>
              <span>{subtotal.toLocaleString()} RWF</span>
            </div>
            <div
              className={`flex justify-between ${
                theme === "dark" ? "text-[#9CA3AF]" : "text-[#6B7280]"
              }`}
            >
              <span>{t.tax}:</span>
              <span>{tax.toLocaleString()} RWF</span>
            </div>
            <div
              className={`border-t-2 pt-2 ${
                theme === "dark" ? "border-[#374151]" : "border-[#E5E7EB]"
              }`}
            >
              <div className="flex justify-between">
                <span
                  className={
                    theme === "dark" ? "text-white" : "text-[#1E3A8A]"
                  }
                >
                  {t.total}:
                </span>
                <span
                  className={
                    theme === "dark" ? "text-white" : "text-[#1E3A8A]"
                  }
                >
                  {total.toLocaleString()} RWF
                </span>
              </div>
            </div>
          </div>

          {/* Payment Methods 
          <div className="mb-4">
            <p
              className={`mb-2 ${
                theme === "dark" ? "text-[#9CA3AF]" : "text-[#6B7280]"
              }`}
            >
              {t.payment}:
            </p>
            <div className="grid grid-cols-3 gap-2">
              <div className="flex h-16 items-center justify-center rounded-lg border-2 border-[#10B981] bg-[#D1FAE5] text-[#10B981]">
                {t.cash}
              </div>
              <div className="flex h-16 items-center justify-center rounded-lg border-2 border-[#3B82F6] bg-[#DBEAFE] text-[#3B82F6]">
                {t.momo}
              </div>
              <div className="flex h-16 items-center justify-center rounded-lg border-2 border-[#F59E0B] bg-[#FEF3C7] text-[#F59E0B]">
                {t.card}
              </div>
            </div>
          </div>*/}

          {/* Process Payment Button */}
          <Button
            onClick={onProcessPayment}
            className="h-16 w-full bg-[#F59E0B] text-white hover:bg-[#D97706]"
          >
            {t.process}
          </Button>
        </>
      )}
    </div>
  );
}