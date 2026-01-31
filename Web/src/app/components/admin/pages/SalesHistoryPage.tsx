import { useState, useEffect } from "react";
import { Card, CardContent } from "@/app/components/ui/card";
import { Button } from "@/app/components/ui/button";
import { RefreshCw, Printer, Eye } from "lucide-react";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/app/components/ui/tabs";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/app/components/ui/table";
import { api, Transaction } from "@/app/lib/api";
import { toast } from "sonner";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/app/components/ui/dialog";
import { Theme } from "@/app/App";

export default function SalesHistoryPage({ theme }: { theme: Theme }) {
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [selectedTransaction, setSelectedTransaction] = useState<Transaction | null>(null);
  const [showDetailsDialog, setShowDetailsDialog] = useState(false);

  useEffect(() => {
    loadTransactions();
  }, []);

  const loadTransactions = async () => {
    try {
      const data = await api.getTransactions();
      setTransactions(data);
    } catch (error) {
      toast.error("Failed to load transactions");
    }
  };

  const handleViewDetails = async (transaction: Transaction) => {
    setSelectedTransaction(transaction);
    setShowDetailsDialog(true);
  };

  const handlePrintReceipt = (transaction: Transaction) => {
    console.log("Print receipt for", transaction.TransactionId);
    toast.success("Printing receipt...");
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className={`mb-2 ${theme === "dark" ? "text-white" : "text-gray-900"}`}>View Sales History</h1>
          <p className={theme === "dark" ? "text-gray-400" : "text-gray-600"}>View and manage all transactions</p>
        </div>
        <Button variant="outline" onClick={loadTransactions}>
          <RefreshCw className="mr-2 h-4 w-4" />
          Refresh
        </Button>
      </div>

      {/* Tabs Layout */}
      <Card className={theme === "dark" ? "border-gray-700 bg-[#1F2937]" : theme === "gray" ? "bg-white" : ""}>
        <CardContent className="p-0">
          <Tabs defaultValue="transactions" className="w-full">
            <div className={`border-b px-6 pt-6 ${theme === "dark" ? "border-gray-700" : ""}`}>
              <TabsList className="grid w-full grid-cols-3">
                <TabsTrigger value="transactions">Transactions</TabsTrigger>
                <TabsTrigger value="details">Transaction Details</TabsTrigger>
                <TabsTrigger value="items">Transaction Items</TabsTrigger>
              </TabsList>
            </div>

            {/* Tab 1: Transactions List with Actions */}
            <TabsContent value="transactions" className="p-6">
              <div className="mb-4 flex gap-2">
                <Button variant="outline" size="sm">
                  <RefreshCw className="mr-2 h-4 w-4" />
                  Refresh
                </Button>
                <Button variant="outline" size="sm">
                  <Printer className="mr-2 h-4 w-4" />
                  Print Receipt
                </Button>
                <Button variant="outline" size="sm">
                  Export
                </Button>
              </div>

              <div className="overflow-x-auto">
                <Table>
                  <TableHeader>
                    <TableRow className={theme === "dark" ? "border-gray-700" : ""}>
                      <TableHead className={theme === "dark" ? "text-gray-300" : ""}>Transaction ID</TableHead>
                      <TableHead className={theme === "dark" ? "text-gray-300" : ""}>Customer</TableHead>
                      <TableHead className={theme === "dark" ? "text-gray-300" : ""}>Date</TableHead>
                      <TableHead className={theme === "dark" ? "text-gray-300" : ""}>Amount</TableHead>
                      <TableHead className={theme === "dark" ? "text-gray-300" : ""}>Payment Method</TableHead>
                      <TableHead className={theme === "dark" ? "text-gray-300" : ""}>Status</TableHead>
                      <TableHead className={theme === "dark" ? "text-gray-300" : ""}>Actions</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {transactions.map((txn) => (
                      <TableRow key={txn.Id} className={theme === "dark" ? "border-gray-700" : ""}>
                        <TableCell className={`font-medium ${theme === "dark" ? "text-gray-300" : ""}`}>{txn.TransactionId}</TableCell>
                        <TableCell className={theme === "dark" ? "text-gray-300" : ""}>{txn.CustomerName}</TableCell>
                        <TableCell className={theme === "dark" ? "text-gray-300" : ""}>
                          {new Date(txn.TransactionDate).toLocaleDateString()}
                        </TableCell>
                        <TableCell className={theme === "dark" ? "text-gray-300" : ""}>RWF {txn.TotalAmount.toLocaleString()}</TableCell>
                        <TableCell className={theme === "dark" ? "text-gray-300" : ""}>{txn.PaymentMethod}</TableCell>
                        <TableCell>
                          <span
                            className={`inline-block rounded-full px-3 py-1 text-xs ${
                              txn.Status === "Completed"
                                ? "bg-green-100 text-green-800"
                                : "bg-yellow-100 text-yellow-800"
                            }`}
                          >
                            {txn.Status}
                          </span>
                        </TableCell>
                        <TableCell>
                          <div className="flex gap-2">
                            <Button
                              size="sm"
                              variant="ghost"
                              onClick={() => handleViewDetails(txn)}
                            >
                              <Eye className="h-4 w-4" />
                            </Button>
                            <Button
                              size="sm"
                              variant="ghost"
                              onClick={() => handlePrintReceipt(txn)}
                            >
                              <Printer className="h-4 w-4" />
                            </Button>
                          </div>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </div>
            </TabsContent>

            {/* Tab 2: Transaction Details */}
            <TabsContent value="details" className="p-6">
              {selectedTransaction ? (
                <div className="space-y-4">
                  <h3 className={`font-semibold ${theme === "dark" ? "text-white" : ""}`}>Transaction Information</h3>
                  <div className="grid gap-4 md:grid-cols-2">
                    <div>
                      <Label className={theme === "dark" ? "text-gray-400" : "text-gray-600"}>Transaction ID</Label>
                      <p className={`font-semibold ${theme === "dark" ? "text-gray-300" : ""}`}>{selectedTransaction.TransactionId}</p>
                    </div>
                    <div>
                      <Label className={theme === "dark" ? "text-gray-400" : "text-gray-600"}>Status</Label>
                      <p className={`font-semibold ${theme === "dark" ? "text-gray-300" : ""}`}>{selectedTransaction.Status}</p>
                    </div>
                    <div>
                      <Label className={theme === "dark" ? "text-gray-400" : "text-gray-600"}>Customer Name</Label>
                      <p className={`font-semibold ${theme === "dark" ? "text-gray-300" : ""}`}>{selectedTransaction.CustomerName}</p>
                    </div>
                    <div>
                      <Label className={theme === "dark" ? "text-gray-400" : "text-gray-600"}>Customer Phone</Label>
                      <p className={`font-semibold ${theme === "dark" ? "text-gray-300" : ""}`}>{selectedTransaction.CustomerPhone}</p>
                    </div>
                    <div>
                      <Label className={theme === "dark" ? "text-gray-400" : "text-gray-600"}>Customer TIN</Label>
                      <p className={`font-semibold ${theme === "dark" ? "text-gray-300" : ""}`}>{selectedTransaction.CustomerTin}</p>
                    </div>
                    <div>
                      <Label className={theme === "dark" ? "text-gray-400" : "text-gray-600"}>Payment Method</Label>
                      <p className={`font-semibold ${theme === "dark" ? "text-gray-300" : ""}`}>{selectedTransaction.PaymentMethod}</p>
                    </div>
                    <div>
                      <Label className={theme === "dark" ? "text-gray-400" : "text-gray-600"}>Subtotal</Label>
                      <p className={`font-semibold ${theme === "dark" ? "text-gray-300" : ""}`}>
                        RWF {selectedTransaction.Subtotal.toLocaleString()}
                      </p>
                    </div>
                    <div>
                      <Label className={theme === "dark" ? "text-gray-400" : "text-gray-600"}>Tax Amount</Label>
                      <p className={`font-semibold ${theme === "dark" ? "text-gray-300" : ""}`}>
                        RWF {selectedTransaction.TaxAmount.toLocaleString()}
                      </p>
                    </div>
                    <div>
                      <Label className={theme === "dark" ? "text-gray-400" : "text-gray-600"}>Total Amount</Label>
                      <p className={`font-semibold ${theme === "dark" ? "text-gray-300" : ""}`}>
                        RWF {selectedTransaction.TotalAmount.toLocaleString()}
                      </p>
                    </div>
                    <div>
                      <Label className={theme === "dark" ? "text-gray-400" : "text-gray-600"}>Transaction Date</Label>
                      <p className={`font-semibold ${theme === "dark" ? "text-gray-300" : ""}`}>
                        {new Date(selectedTransaction.TransactionDate).toLocaleString()}
                      </p>
                    </div>
                  </div>
                </div>
              ) : (
                <div className={`py-12 text-center ${theme === "dark" ? "text-gray-400" : "text-gray-500"}`}>
                  Select a transaction to view details
                </div>
              )}
            </TabsContent>

            {/* Tab 3: Transaction Items */}
            <TabsContent value="items" className="p-6">
              {selectedTransaction ? (
                <div className="space-y-4">
                  <h3 className={`font-semibold ${theme === "dark" ? "text-white" : ""}`}>Transaction Items</h3>
                  <div className="overflow-x-auto">
                    <table className="w-full">
                      <thead>
                        <tr className={`border-b ${theme === "dark" ? "border-gray-700" : ""}`}>
                          <th className={`pb-3 text-left ${theme === "dark" ? "text-gray-300" : ""}`}>Service</th>
                          <th className={`pb-3 text-right ${theme === "dark" ? "text-gray-300" : ""}`}>Quantity</th>
                          <th className={`pb-3 text-right ${theme === "dark" ? "text-gray-300" : ""}`}>Unit Price</th>
                          <th className={`pb-3 text-right ${theme === "dark" ? "text-gray-300" : ""}`}>Total</th>
                        </tr>
                      </thead>
                      <tbody>
                        <tr className={`border-b ${theme === "dark" ? "border-gray-700" : ""}`}>
                          <td className={`py-3 ${theme === "dark" ? "text-gray-300" : ""}`}>Sample Service</td>
                          <td className={`py-3 text-right ${theme === "dark" ? "text-gray-300" : ""}`}>1</td>
                          <td className={`py-3 text-right ${theme === "dark" ? "text-gray-300" : ""}`}>
                            RWF {selectedTransaction.Subtotal.toLocaleString()}
                          </td>
                          <td className={`py-3 text-right ${theme === "dark" ? "text-gray-300" : ""}`}>
                            RWF {selectedTransaction.Subtotal.toLocaleString()}
                          </td>
                        </tr>
                      </tbody>
                      <tfoot>
                        <tr className={`border-t-2 font-bold ${theme === "dark" ? "border-gray-600" : ""}`}>
                          <td className={`pt-3 ${theme === "dark" ? "text-gray-300" : ""}`} colSpan={3}>
                            Total
                          </td>
                          <td className={`pt-3 text-right ${theme === "dark" ? "text-gray-300" : ""}`}>
                            RWF {selectedTransaction.TotalAmount.toLocaleString()}
                          </td>
                        </tr>
                      </tfoot>
                    </table>
                  </div>
                </div>
              ) : (
                <div className={`py-12 text-center ${theme === "dark" ? "text-gray-400" : "text-gray-500"}`}>
                  Select a transaction to view items
                </div>
              )}
            </TabsContent>
          </Tabs>
        </CardContent>
      </Card>
    </div>
  );
}

function Label({ children, className = "" }: { children: React.ReactNode; className?: string }) {
  return <label className={`text-sm font-medium ${className}`}>{children}</label>;
}