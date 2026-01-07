import { useState } from "react";
import { User, Transaction, Screen } from "../App";
import Header from "./Header";
import Footer from "./Footer";
import { Input } from "./ui/input";
import { Button } from "./ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "./ui/select";
import {
  Search,
  CheckCircle,
  AlertCircle,
  Clock,
  Eye,
  RotateCcw,
  RefreshCw,
} from "lucide-react";

interface TransactionHistoryProps {
  user: User;
  transactions: Transaction[];
  onNavigate: (screen: Screen) => void;
  onLogout: () => void;
}

export default function TransactionHistory({
  user,
  transactions,
  onNavigate,
  onLogout,
}: TransactionHistoryProps) {
  const [searchQuery, setSearchQuery] = useState("");
  const [statusFilter, setStatusFilter] = useState("all");
  const [dateFilter, setDateFilter] = useState("today");

  const getStatusIcon = (status: string) => {
    switch (status) {
      case "completed":
        return <CheckCircle className="h-5 w-5 text-[#10B981]" />;
      case "failed":
        return <AlertCircle className="h-5 w-5 text-[#EF4444]" />;
      case "pending":
        return <Clock className="h-5 w-5 text-[#F59E0B]" />;
      default:
        return null;
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case "completed":
        return <span className="text-[#10B981]">✓ Completed</span>;
      case "failed":
        return <span className="text-[#EF4444]">✗ Failed</span>;
      case "pending":
        return <span className="text-[#F59E0B]">⚡ Pending</span>;
      default:
        return status;
    }
  };

  const filteredTransactions = transactions.filter((transaction) => {
    const matchesSearch =
      transaction.id.toLowerCase().includes(searchQuery.toLowerCase()) ||
      transaction.customer?.includes(searchQuery);
    const matchesStatus =
      statusFilter === "all" || transaction.status === statusFilter;
    return matchesSearch && matchesStatus;
  });

  return (
    <div className="flex h-full flex-col">
      <Header user={user} />

      <div className="flex-1 overflow-hidden p-6">
        <div className="h-full rounded-xl border-2 border-[#E5E7EB] bg-white p-6 shadow-lg">
          {/* Header */}
          <div className="mb-6 border-b-2 border-[#E5E7EB] pb-4">
            <h2 className="text-[#1E3A8A]">Transaction History</h2>
          </div>

          {/* Filters */}
          <div className="mb-6 grid grid-cols-3 gap-4">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 h-5 w-5 -translate-y-1/2 text-[#6B7280]" />
              <Input
                placeholder="Search by ID or Customer..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="h-12 border-2 pl-10"
              />
            </div>

            <Select value={dateFilter} onValueChange={setDateFilter}>
              <SelectTrigger className="h-12 border-2">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="today">Today</SelectItem>
                <SelectItem value="week">This Week</SelectItem>
                <SelectItem value="month">This Month</SelectItem>
                <SelectItem value="all">All Time</SelectItem>
              </SelectContent>
            </Select>

            <div className="flex gap-2">
              <Button
                variant={statusFilter === "all" ? "default" : "outline"}
                onClick={() => setStatusFilter("all")}
                className={`flex-1 h-12 ${
                  statusFilter === "all"
                    ? "bg-[#1E3A8A]"
                    : "border-2"
                }`}
              >
                All
              </Button>
              <Button
                variant={statusFilter === "completed" ? "default" : "outline"}
                onClick={() => setStatusFilter("completed")}
                className={`flex-1 h-12 ${
                  statusFilter === "completed"
                    ? "bg-[#10B981] hover:bg-[#059669]"
                    : "border-2"
                }`}
              >
                Completed
              </Button>
              <Button
                variant={statusFilter === "pending" ? "default" : "outline"}
                onClick={() => setStatusFilter("pending")}
                className={`flex-1 h-12 ${
                  statusFilter === "pending"
                    ? "bg-[#F59E0B] hover:bg-[#D97706]"
                    : "border-2"
                }`}
              >
                Pending
              </Button>
              <Button
                variant={statusFilter === "failed" ? "default" : "outline"}
                onClick={() => setStatusFilter("failed")}
                className={`flex-1 h-12 ${
                  statusFilter === "failed"
                    ? "bg-[#EF4444] hover:bg-[#DC2626]"
                    : "border-2"
                }`}
              >
                Failed
              </Button>
            </div>
          </div>

          {/* Transaction Table */}
          <div className="mb-4 overflow-hidden rounded-lg border-2 border-[#E5E7EB]">
            <table className="w-full">
              <thead className="bg-[#F9FAFB]">
                <tr>
                  <th className="px-4 py-3 text-left text-[#6B7280]">ID</th>
                  <th className="px-4 py-3 text-left text-[#6B7280]">TIME</th>
                  <th className="px-4 py-3 text-left text-[#6B7280]">AMOUNT</th>
                  <th className="px-4 py-3 text-left text-[#6B7280]">
                    PAYMENT
                  </th>
                  <th className="px-4 py-3 text-left text-[#6B7280]">
                    CUSTOMER
                  </th>
                  <th className="px-4 py-3 text-left text-[#6B7280]">STATUS</th>
                  <th className="px-4 py-3 text-left text-[#6B7280]">
                    ACTIONS
                  </th>
                </tr>
              </thead>
              <tbody>
                {filteredTransactions.length === 0 ? (
                  <tr>
                    <td colSpan={7} className="py-12 text-center">
                      <p className="text-[#6B7280]">No transactions found</p>
                    </td>
                  </tr>
                ) : (
                  filteredTransactions.map((transaction) => (
                    <tr
                      key={transaction.id}
                      className="border-t border-[#E5E7EB] hover:bg-[#F9FAFB]"
                    >
                      <td className="px-4 py-3 text-[#1E3A8A]">
                        {transaction.id}
                      </td>
                      <td className="px-4 py-3 text-[#6B7280]">
                        {new Date(transaction.timestamp).toLocaleTimeString(
                          "en-US",
                          {
                            hour: "2-digit",
                            minute: "2-digit",
                            hour12: false,
                          }
                        )}
                      </td>
                      <td className="px-4 py-3 text-[#F59E0B]">
                        {transaction.amount.toLocaleString()} RWF
                      </td>
                      <td className="px-4 py-3 text-[#6B7280]">
                        {transaction.paymentMethod}
                      </td>
                      <td className="px-4 py-3 text-[#6B7280]">
                        {transaction.customer || "-"}
                      </td>
                      <td className="px-4 py-3">
                        <div className="flex items-center gap-2">
                          {getStatusIcon(transaction.status)}
                          {getStatusText(transaction.status)}
                        </div>
                      </td>
                      <td className="px-4 py-3">
                        <div className="flex gap-2">
                          <Button
                            variant="ghost"
                            size="sm"
                            className="h-8 text-[#1E3A8A] hover:bg-[#DBEAFE]"
                          >
                            <Eye className="h-4 w-4" />
                          </Button>
                          {transaction.status === "failed" && (
                            <Button
                              variant="ghost"
                              size="sm"
                              className="h-8 text-[#F59E0B] hover:bg-[#FEF3C7]"
                            >
                              <RotateCcw className="h-4 w-4" />
                            </Button>
                          )}
                          {transaction.status === "pending" && (
                            <Button
                              variant="ghost"
                              size="sm"
                              className="h-8 text-[#10B981] hover:bg-[#D1FAE5]"
                            >
                              <RefreshCw className="h-4 w-4" />
                            </Button>
                          )}
                        </div>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>

          {/* Pagination */}
          {filteredTransactions.length > 0 && (
            <div className="flex items-center justify-between">
              <p className="text-[#6B7280]">
                Showing 1-{filteredTransactions.length} of{" "}
                {filteredTransactions.length} transactions
              </p>
              <div className="flex gap-2">
                <Button variant="outline" className="h-10 border-2">
                  ← Previous
                </Button>
                <Button variant="outline" className="h-10 border-2 bg-[#1E3A8A] text-white">
                  1
                </Button>
                <Button variant="outline" className="h-10 border-2">
                  Next →
                </Button>
              </div>
            </div>
          )}
        </div>
      </div>

      <Footer
        currentScreen="history"
        onNavigate={onNavigate}
        onLogout={onLogout}
      />
    </div>
  );
}
