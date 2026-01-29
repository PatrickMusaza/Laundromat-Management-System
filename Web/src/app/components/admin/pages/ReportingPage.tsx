import { useState } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/app/components/ui/card";
import { Button } from "@/app/components/ui/button";
import { FileDown, Printer, FileSpreadsheet } from "lucide-react";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/app/components/ui/select";
import { Label } from "@/app/components/ui/label";

export default function ReportingPage() {
  const [reportType, setReportType] = useState("services");
  const [dateFilter, setDateFilter] = useState("today");

  const handleExportPDF = () => {
    console.log("Export PDF");
  };

  const handleExportExcel = () => {
    console.log("Export Excel");
  };

  const handlePrint = () => {
    window.print();
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="mb-2">Reporting</h1>
          <p className="text-gray-600">Generate and export various reports</p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={handlePrint}>
            <Printer className="mr-2 h-4 w-4" />
            Print
          </Button>
          <Button variant="outline" onClick={handleExportExcel}>
            <FileSpreadsheet className="mr-2 h-4 w-4" />
            Excel
          </Button>
          <Button variant="outline" onClick={handleExportPDF}>
            <FileDown className="mr-2 h-4 w-4" />
            PDF
          </Button>
        </div>
      </div>

      {/* Filters */}
      <Card>
        <CardHeader>
          <CardTitle>Report Filters</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid gap-4 md:grid-cols-3">
            <div>
              <Label>Report Type</Label>
              <Select value={reportType} onValueChange={setReportType}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="services">Services</SelectItem>
                  <SelectItem value="service-category">Service Category</SelectItem>
                  <SelectItem value="users">Users</SelectItem>
                  <SelectItem value="payment-types">Payment Types</SelectItem>
                  <SelectItem value="payment-types-by-users">
                    Payment Types by Users
                  </SelectItem>
                  <SelectItem value="daily-sales">Daily Sales</SelectItem>
                  <SelectItem value="hourly-sales">Hourly Sales</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div>
              <Label>Date Range</Label>
              <Select value={dateFilter} onValueChange={setDateFilter}>
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="today">Today</SelectItem>
                  <SelectItem value="yesterday">Yesterday</SelectItem>
                  <SelectItem value="this-week">This Week</SelectItem>
                  <SelectItem value="last-week">Last Week</SelectItem>
                  <SelectItem value="this-month">This Month</SelectItem>
                  <SelectItem value="last-month">Last Month</SelectItem>
                  <SelectItem value="this-year">This Year</SelectItem>
                  <SelectItem value="last-year">Last Year</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="flex items-end">
              <Button className="w-full">Generate Report</Button>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Report Results */}
      <Card>
        <CardHeader>
          <CardTitle>
            {reportType === "services" && "Services Report"}
            {reportType === "service-category" && "Service Category Report"}
            {reportType === "users" && "Users Report"}
            {reportType === "payment-types" && "Payment Types Report"}
            {reportType === "payment-types-by-users" && "Payment Types by Users Report"}
            {reportType === "daily-sales" && "Daily Sales Report"}
            {reportType === "hourly-sales" && "Hourly Sales Report"}
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b">
                  <th className="pb-3 text-left">Item</th>
                  <th className="pb-3 text-right">Count</th>
                  <th className="pb-3 text-right">Amount</th>
                  <th className="pb-3 text-right">%</th>
                </tr>
              </thead>
              <tbody>
                {[1, 2, 3, 4, 5].map((i) => (
                  <tr key={i} className="border-b">
                    <td className="py-3">Item {i}</td>
                    <td className="py-3 text-right">{i * 10}</td>
                    <td className="py-3 text-right">
                      RWF {(i * 10000).toLocaleString()}
                    </td>
                    <td className="py-3 text-right">{i * 5}%</td>
                  </tr>
                ))}
              </tbody>
              <tfoot>
                <tr className="border-t-2 font-bold">
                  <td className="pt-3">Total</td>
                  <td className="pt-3 text-right">150</td>
                  <td className="pt-3 text-right">RWF 150,000</td>
                  <td className="pt-3 text-right">100%</td>
                </tr>
              </tfoot>
            </table>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
