import { useEffect, useState } from "react";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
} from "recharts";
import { Card, CardContent, CardHeader, CardTitle } from "@/app/components/ui/card";
import { DollarSign, Package, Users, TrendingUp } from "lucide-react";

const COLORS = ["#1E3A8A", "#F59E0B", "#10B981", "#EF4444", "#8B5CF6"];

export default function AdminDashboard() {
  const [monthlySales, setMonthlySales] = useState([
    { month: "Jan", sales: 45000 },
    { month: "Feb", sales: 52000 },
    { month: "Mar", sales: 48000 },
    { month: "Apr", sales: 61000 },
    { month: "May", sales: 55000 },
    { month: "Jun", sales: 67000 },
  ]);

  const [topServices, setTopServices] = useState([
    { name: "Hot Water Wash", value: 3500 },
    { name: "Cold Water Wash", value: 2800 },
    { name: "Express Dry", value: 2200 },
    { name: "Premium Detergent", value: 1500 },
    { name: "Ironing", value: 1200 },
  ]);

  const stats = [
    {
      title: "Total Revenue",
      value: "RWF 328,000",
      change: "+12.5%",
      icon: DollarSign,
      color: "text-green-600",
      bgColor: "bg-green-100",
    },
    {
      title: "Total Orders",
      value: "1,234",
      change: "+8.2%",
      icon: Package,
      color: "text-blue-600",
      bgColor: "bg-blue-100",
    },
    {
      title: "Active Customers",
      value: "892",
      change: "+5.7%",
      icon: Users,
      color: "text-purple-600",
      bgColor: "bg-purple-100",
    },
    {
      title: "Growth Rate",
      value: "15.3%",
      change: "+2.4%",
      icon: TrendingUp,
      color: "text-orange-600",
      bgColor: "bg-orange-100",
    },
  ];

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="mb-2">Dashboard</h1>
        <p className="text-gray-600">Welcome back! Here's an overview of your laundromat business.</p>
      </div>

      {/* Stats Grid */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        {stats.map((stat, index) => {
          const Icon = stat.icon;
          return (
            <Card key={index}>
              <CardContent className="p-6">
                <div className="flex items-center justify-between">
                  <div>
                    <p className="text-sm text-gray-600">{stat.title}</p>
                    <h3 className="mt-1">{stat.value}</h3>
                    <p className={`mt-1 text-sm ${stat.color}`}>{stat.change} from last month</p>
                  </div>
                  <div className={`rounded-full p-3 ${stat.bgColor}`}>
                    <Icon className={`h-6 w-6 ${stat.color}`} />
                  </div>
                </div>
              </CardContent>
            </Card>
          );
        })}
      </div>

      {/* Charts */}
      <div className="grid gap-6 md:grid-cols-2">
        {/* Monthly Sales Histogram */}
        <Card>
          <CardHeader>
            <CardTitle>Monthly Sales</CardTitle>
          </CardHeader>
          <CardContent>
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={monthlySales}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="month" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Bar dataKey="sales" fill="#1E3A8A" />
              </BarChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>

        {/* Top Services Pie Chart */}
        <Card>
          <CardHeader>
            <CardTitle>Top Services</CardTitle>
          </CardHeader>
          <CardContent>
            <ResponsiveContainer width="100%" height={300}>
              <PieChart>
                <Pie
                  data={topServices}
                  cx="50%"
                  cy="50%"
                  labelLine={false}
                  label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                  outerRadius={80}
                  fill="#8884d8"
                  dataKey="value"
                >
                  {topServices.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip />
              </PieChart>
            </ResponsiveContainer>
          </CardContent>
        </Card>
      </div>

      {/* Recent Transactions */}
      <Card>
        <CardHeader>
          <CardTitle>Recent Transactions</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead>
                <tr className="border-b">
                  <th className="pb-3 text-left">Transaction ID</th>
                  <th className="pb-3 text-left">Customer</th>
                  <th className="pb-3 text-left">Amount</th>
                  <th className="pb-3 text-left">Payment Method</th>
                  <th className="pb-3 text-left">Status</th>
                </tr>
              </thead>
              <tbody>
                {[1, 2, 3, 4, 5].map((i) => (
                  <tr key={i} className="border-b last:border-0">
                    <td className="py-3">TXN-{String(i).padStart(6, "0")}</td>
                    <td className="py-3">Customer {i}</td>
                    <td className="py-3">RWF {(5000 + i * 1000).toLocaleString()}</td>
                    <td className="py-3">{["Cash", "MoMo", "Card"][i % 3]}</td>
                    <td className="py-3">
                      <span className="inline-block rounded-full bg-green-100 px-3 py-1 text-xs text-green-800">
                        Completed
                      </span>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
