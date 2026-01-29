import { useState } from "react";
import { Theme } from "../App";
import { Button } from "./ui/button";
import { Input } from "./ui/input";
import { Label } from "./ui/label";
import { X, Banknote, Smartphone, CreditCard, QrCode } from "lucide-react";
import { toast } from "sonner";

interface PaymentModalProps {
  total: number;
  onClose: () => void;
  onPaymentComplete: (
    paymentMethod: "Cash" | "MoMo" | "Card",
    customer?: string
  ) => void;
  transactionId: string;
  language: "EN" | "RW" | "FR";
  theme: Theme;
}

const translations = {
  EN: {
    title: "Payment Processing",
    transaction: "Transaction #",
    total: "Total",
    customer: "Customer",
    cash: "CASH",
    momo: "MOBILE MONEY",
    card: "CARD PAYMENT",
    enterAmount: "Enter Amount",
    received: "Cash Received",
    change: "Change Due",
    process: "Process Cash",
    generateQR: "Generate QR Code",
    confirm: "Confirm Payment",
    swipeCard: "Swipe/Tap Card",
    processCard: "Process Card",
    cancel: "Cancel",
    print: "Print Receipt",
    processing: "Processing...",
    success: "Payment Successful!",
  },
  RW: {
    title: "Gutanga Amafaranga",
    transaction: "Ikirangirizo #",
    total: "Igiteranyo",
    customer: "Umukiriya",
    cash: "AMAFARANGA",
    momo: "MOMO",
    card: "KARITA",
    enterAmount: "Andika Amafaranga",
    received: "Amafaranga Yakiriye",
    change: "Amafaranga Yo Gusubiza",
    process: "Emeza Amafaranga",
    generateQR: "Kora QR Code",
    confirm: "Emeza Kwishyura",
    swipeCard: "Koresha Karita",
    processCard: "Emeza Karita",
    cancel: "Hagarika",
    print: "Funga Inyemezabwishyu",
    processing: "Gutunganya...",
    success: "Kwishyura Byagenze Neza!",
  },
  FR: {
    title: "Traitement du Paiement",
    transaction: "Transaction #",
    total: "Total",
    customer: "Client",
    cash: "ESPÈCES",
    momo: "MOBILE MONEY",
    card: "PAIEMENT CARTE",
    enterAmount: "Entrer le Montant",
    received: "Espèces Reçues",
    change: "Monnaie à Rendre",
    process: "Traiter Espèces",
    generateQR: "Générer QR Code",
    confirm: "Confirmer Paiement",
    swipeCard: "Glisser/Taper Carte",
    processCard: "Traiter Carte",
    cancel: "Annuler",
    print: "Imprimer Reçu",
    processing: "Traitement...",
    success: "Paiement Réussi!",
  },
};

export default function PaymentModal({
  total,
  onClose,
  onPaymentComplete,
  transactionId,
  language,
  theme,
}: PaymentModalProps) {
  const [selectedMethod, setSelectedMethod] = useState<
    "Cash" | "MoMo" | "Card" | null
  >(null);
  const [cashReceived, setCashReceived] = useState("");
  const [customer, setCustomer] = useState("");
  const [processing, setProcessing] = useState(false);

  const t = translations[language];

  const change = cashReceived
    ? Math.max(0, parseFloat(cashReceived) - total)
    : 0;

  const handleNumPadClick = (value: string) => {
    if (value === "Clear") {
      setCashReceived("");
    } else {
      setCashReceived((prev) => prev + value);
    }
  };

  const handleProcessPayment = async () => {
    if (!selectedMethod) return;

    if (selectedMethod === "Cash") {
      const received = parseFloat(cashReceived);
      if (!received || received < total) {
        toast.error("Insufficient cash amount");
        return;
      }
    }

    setProcessing(true);
    // Simulate payment processing
    await new Promise((resolve) => setTimeout(resolve, 1500));

    toast.success(t.success);
    onPaymentComplete(selectedMethod, customer || undefined);
    setProcessing(false);
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
      <div
        className={`relative w-full max-w-4xl rounded-xl p-8 shadow-2xl ${
          theme === "dark"
            ? "bg-[#1F2937]"
            : theme === "gray"
            ? "bg-white"
            : "bg-white"
        }`}
      >
        {/* Header */}
        <div
          className={`mb-6 flex items-center justify-between border-b-2 pb-4 ${
            theme === "dark" ? "border-[#374151]" : "border-[#E5E7EB]"
          }`}
        >
          <div>
            <h2
              className={
                theme === "dark"
                  ? "text-white"
                  : "text-[#1E3A8A]"
              }
            >
              {t.title}
            </h2>
            <p
              className={
                theme === "dark"
                  ? "text-[#9CA3AF]"
                  : "text-[#6B7280]"
              }
            >
              {t.transaction}: {transactionId} | {t.total}:{" "}
              <span className="text-[#F59E0B]">{total.toLocaleString()} RWF</span>
            </p>
          </div>
          <button
            onClick={onClose}
            className={`rounded-lg p-2 ${
              theme === "dark" ? "hover:bg-[#374151]" : "hover:bg-[#F9FAFB]"
            }`}
          >
            <X
              className={`h-6 w-6 ${
                theme === "dark" ? "text-white" : ""
              }`}
            />
          </button>
        </div>

        {/* Customer Input */}
        <div className="mb-6">
          <Label
            htmlFor="customer"
            className={theme === "dark" ? "text-white" : ""}
          >
            {t.customer} (Optional)
          </Label>
          <Input
            id="customer"
            type="tel"
            placeholder="+250 788 123 456"
            value={customer}
            onChange={(e) => setCustomer(e.target.value)}
            className={`h-12 border-2 ${
              theme === "dark"
                ? "border-[#374151] bg-[#111827] text-white"
                : ""
            }`}
          />
        </div>

        {/* Payment Method Selection */}
        {!selectedMethod ? (
          <div className="grid grid-cols-3 gap-4">
            <button
              onClick={() => setSelectedMethod("Cash")}
              className="flex h-48 flex-col items-center justify-center gap-4 rounded-xl border-2 border-[#10B981] bg-[#D1FAE5] transition-all hover:shadow-lg"
            >
              <Banknote className="h-16 w-16 text-[#10B981]" />
              <span className="text-[#10B981]">{t.cash}</span>
            </button>

            <button
              onClick={() => setSelectedMethod("MoMo")}
              className="flex h-48 flex-col items-center justify-center gap-4 rounded-xl border-2 border-[#3B82F6] bg-[#DBEAFE] transition-all hover:shadow-lg"
            >
              <Smartphone className="h-16 w-16 text-[#3B82F6]" />
              <span className="text-[#3B82F6]">{t.momo}</span>
            </button>

            <button
              onClick={() => setSelectedMethod("Card")}
              className="flex h-48 flex-col items-center justify-center gap-4 rounded-xl border-2 border-[#F59E0B] bg-[#FEF3C7] transition-all hover:shadow-lg"
            >
              <CreditCard className="h-16 w-16 text-[#F59E0B]" />
              <span className="text-[#F59E0B]">{t.card}</span>
            </button>
          </div>
        ) : (
          <>
            {/* Cash Payment */}
            {selectedMethod === "Cash" && (
              <div className="grid grid-cols-2 gap-6">
                <div className="space-y-4">
                  <div>
                    <Label className={theme === "dark" ? "text-white" : ""}>
                      {t.total}
                    </Label>
                    <div
                      className={`mt-2 rounded-lg border-2 p-4 ${
                        theme === "dark"
                          ? "border-[#374151] bg-[#111827]"
                          : "border-[#E5E7EB] bg-[#F9FAFB]"
                      }`}
                    >
                      <p className="text-[#F59E0B]">
                        {total.toLocaleString()} RWF
                      </p>
                    </div>
                  </div>

                  <div>
                    <Label className={theme === "dark" ? "text-white" : ""}>
                      {t.received}
                    </Label>
                    <Input
                      value={cashReceived}
                      readOnly
                      className={`mt-2 h-16 border-2 text-[#1E3A8A] ${
                        theme === "dark"
                          ? "border-[#374151] bg-[#111827] text-white"
                          : ""
                      }`}
                    />
                  </div>

                  <div>
                    <Label className={theme === "dark" ? "text-white" : ""}>
                      {t.change}
                    </Label>
                    <div className="mt-2 rounded-lg border-2 border-[#10B981] bg-[#D1FAE5] p-4">
                      <p className="text-[#10B981]">
                        {change.toLocaleString()} RWF
                      </p>
                    </div>
                  </div>
                </div>

                {/* Number Pad */}
                <div className="grid grid-cols-3 gap-2">
                  {["1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "00", "Clear"].map(
                    (num) => (
                      <Button
                        key={num}
                        onClick={() => handleNumPadClick(num)}
                        variant="outline"
                        className={`h-16 border-2 ${
                          num === "Clear"
                            ? "bg-[#EF4444] text-white hover:bg-[#DC2626]"
                            : theme === "dark"
                            ? "bg-[#374151] text-white hover:bg-[#4B5563]"
                            : "bg-white text-[#1E3A8A] hover:bg-[#F9FAFB]"
                        }`}
                      >
                        {num}
                      </Button>
                    )
                  )}
                </div>
              </div>
            )}

            {/* Mobile Money Payment */}
            {selectedMethod === "MoMo" && (
              <div className="flex flex-col items-center gap-6">
                <div className="flex h-64 w-64 items-center justify-center rounded-xl border-4 border-[#3B82F6] bg-white">
                  <QrCode className="h-48 w-48 text-[#3B82F6]" />
                </div>
                <p
                  className={
                    theme === "dark"
                      ? "text-[#9CA3AF]"
                      : "text-[#6B7280]"
                  }
                >
                  Scan QR Code with Mobile Money App
                </p>
                <p
                  className={
                    theme === "dark"
                      ? "text-white"
                      : "text-[#1E3A8A]"
                  }
                >
                  {total.toLocaleString()} RWF
                </p>
              </div>
            )}

            {/* Card Payment */}
            {selectedMethod === "Card" && (
              <div className="flex flex-col items-center gap-6">
                <div className="flex h-48 w-full items-center justify-center rounded-xl border-2 border-[#F59E0B] bg-[#FEF3C7]">
                  <div className="text-center">
                    <CreditCard className="mx-auto mb-4 h-16 w-16 text-[#F59E0B]" />
                    <p className="text-[#F59E0B]">{t.swipeCard}</p>
                  </div>
                </div>
                <p
                  className={
                    theme === "dark"
                      ? "text-white"
                      : "text-[#1E3A8A]"
                  }
                >
                  {total.toLocaleString()} RWF
                </p>
              </div>
            )}

            {/* Action Buttons */}
            <div className="mt-6 flex gap-4">
              <Button
                onClick={() => setSelectedMethod(null)}
                variant="outline"
                className="h-14 flex-1 border-2"
              >
                {t.cancel}
              </Button>
              <Button
                onClick={handleProcessPayment}
                disabled={processing || (selectedMethod === "Cash" && !cashReceived)}
                className="h-14 flex-1 bg-[#10B981] hover:bg-[#059669]"
              >
                {processing ? t.processing : t.process}
              </Button>
            </div>
          </>
        )}
      </div>
    </div>
  );
}