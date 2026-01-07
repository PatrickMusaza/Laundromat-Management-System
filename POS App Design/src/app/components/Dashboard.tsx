import { useState } from "react";
import { CartItem, Transaction, Theme } from "../App";
import CustomerHeader from "./CustomerHeader";
import ServiceGrid from "./ServiceGrid";
import ShoppingCart from "./ShoppingCart";
import PaymentModal from "./PaymentModal";

interface DashboardProps {
  onAddTransaction: (transaction: Transaction) => void;
  language: "EN" | "RW" | "FR";
  onLanguageChange: (lang: "EN" | "RW" | "FR") => void;
  theme: Theme;
  onThemeChange: (theme: Theme) => void;
}

export default function Dashboard({
  onAddTransaction,
  language,
  onLanguageChange,
  theme,
  onThemeChange,
}: DashboardProps) {
  const [cart, setCart] = useState<CartItem[]>([]);
  const [showPayment, setShowPayment] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState<string>("washing");

  const handleAddToCart = (item: CartItem) => {
    setCart((prev) => {
      const existing = prev.find(
        (i) => i.name === item.name && JSON.stringify(i.addons) === JSON.stringify(item.addons)
      );
      if (existing) {
        return prev.map((i) =>
          i.name === item.name && JSON.stringify(i.addons) === JSON.stringify(item.addons)
            ? { ...i, quantity: i.quantity + 1 }
            : i
        );
      }
      return [...prev, { ...item, quantity: 1 }];
    });
  };

  const handleRemoveFromCart = (itemId: string) => {
    setCart((prev) => prev.filter((item) => item.id !== itemId));
  };

  const handleUpdateQuantity = (itemId: string, quantity: number) => {
    if (quantity <= 0) {
      handleRemoveFromCart(itemId);
      return;
    }
    setCart((prev) =>
      prev.map((item) => (item.id === itemId ? { ...item, quantity } : item))
    );
  };

  const handleProcessPayment = () => {
    if (cart.length === 0) return;
    setShowPayment(true);
  };

  const handlePaymentComplete = (
    paymentMethod: "Cash" | "MoMo" | "Card",
    customer?: string
  ) => {
    const transaction: Transaction = {
      id: `T-${Date.now().toString().slice(-6)}`,
      timestamp: new Date().toISOString(),
      amount: calculateTotal(),
      paymentMethod,
      status: "completed",
      items: [...cart],
      customer,
    };

    onAddTransaction(transaction);
    setCart([]);
    setShowPayment(false);
  };

  const calculateSubtotal = () => {
    return cart.reduce((sum, item) => {
      const itemTotal =
        item.price + item.addons.reduce((a, addon) => a + addon.price, 0);
      return sum + itemTotal * item.quantity;
    }, 0);
  };

  const calculateTax = () => {
    return Math.round(calculateSubtotal() * 0.1);
  };

  const calculateTotal = () => {
    return calculateSubtotal() + calculateTax();
  };

  return (
    <div className="flex h-full flex-col">
      <CustomerHeader
        language={language}
        onLanguageChange={onLanguageChange}
        theme={theme}
        onThemeChange={onThemeChange}
      />

      <div className="flex flex-1 gap-6 overflow-hidden p-6">
        {/* Left Panel - Services */}
        <div className="flex flex-[7] flex-col gap-4 overflow-hidden">
          <ServiceGrid
            selectedCategory={selectedCategory}
            onCategoryChange={setSelectedCategory}
            onAddToCart={handleAddToCart}
            language={language}
            theme={theme}
          />
        </div>

        {/* Right Panel - Shopping Cart */}
        <div className="flex-[3]">
          <ShoppingCart
            cart={cart}
            onRemoveItem={handleRemoveFromCart}
            onUpdateQuantity={handleUpdateQuantity}
            onProcessPayment={handleProcessPayment}
            subtotal={calculateSubtotal()}
            tax={calculateTax()}
            total={calculateTotal()}
            language={language}
            theme={theme}
          />
        </div>
      </div>

      {showPayment && (
        <PaymentModal
          total={calculateTotal()}
          onClose={() => setShowPayment(false)}
          onPaymentComplete={handlePaymentComplete}
          transactionId={`T-${Date.now().toString().slice(-6)}`}
          language={language}
          theme={theme}
        />
      )}
    </div>
  );
}
