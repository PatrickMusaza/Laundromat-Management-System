import { useState } from "react";
import { User, Screen } from "../App";
import Header from "./Header";
import Footer from "./Footer";
import { Button } from "./ui/button";
import { Input } from "./ui/input";
import { Label } from "./ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "./ui/select";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "./ui/tabs";
import { toast } from "sonner";

interface SettingsProps {
  user: User;
  onNavigate: (screen: Screen) => void;
  onLogout: () => void;
  language: "EN" | "RW" | "FR";
  onLanguageChange: (lang: "EN" | "RW" | "FR") => void;
}

export default function Settings({
  user,
  onNavigate,
  onLogout,
  language,
  onLanguageChange,
}: SettingsProps) {
  const [settingsLanguage, setSettingsLanguage] = useState(language);
  const [currency, setCurrency] = useState("RWF");
  const [timezone, setTimezone] = useState("Africa/Kigali");
  const [dateFormat, setDateFormat] = useState("DD/MM/YYYY");
  const [receiptHeader, setReceiptHeader] = useState("Laundromat System");
  const [receiptFooter, setReceiptFooter] = useState(
    "Thank you for your business!"
  );

  // Price Management
  const [prices, setPrices] = useState([
    { service: "Hot Water", current: 5000, new: "" },
    { service: "Cold Water", current: 6000, new: "" },
    { service: "Ironing", current: 1000, new: "" },
    { service: "Regular Dry", current: 3000, new: "" },
    { service: "Heavy Duty Dry", current: 5000, new: "" },
  ]);

  const handleSaveSettings = () => {
    onLanguageChange(settingsLanguage);
    toast.success("Settings saved successfully!");
  };

  const handleSavePrices = () => {
    toast.success("Price changes scheduled!");
  };

  const handleResetDefaults = () => {
    setSettingsLanguage("EN");
    setCurrency("RWF");
    setTimezone("Africa/Kigali");
    setDateFormat("DD/MM/YYYY");
    setReceiptHeader("Laundromat System");
    setReceiptFooter("Thank you for your business!");
    toast.info("Settings reset to defaults");
  };

  return (
    <div className="flex h-full flex-col">
      <Header user={user} />

      <div className="flex-1 overflow-hidden p-6">
        <div className="h-full rounded-xl border-2 border-[#E5E7EB] bg-white shadow-lg">
          <Tabs defaultValue="general" className="h-full flex flex-col">
            <TabsList className="border-b-2 border-[#E5E7EB] rounded-none bg-[#F9FAFB] p-0 h-auto">
              <TabsTrigger
                value="general"
                className="h-14 rounded-none border-b-2 border-transparent data-[state=active]:border-[#1E3A8A] data-[state=active]:bg-white data-[state=active]:text-[#1E3A8A]"
              >
                General
              </TabsTrigger>
              <TabsTrigger
                value="prices"
                className="h-14 rounded-none border-b-2 border-transparent data-[state=active]:border-[#1E3A8A] data-[state=active]:bg-white data-[state=active]:text-[#1E3A8A]"
              >
                Prices
              </TabsTrigger>
              <TabsTrigger
                value="users"
                className="h-14 rounded-none border-b-2 border-transparent data-[state=active]:border-[#1E3A8A] data-[state=active]:bg-white data-[state=active]:text-[#1E3A8A]"
              >
                Users
              </TabsTrigger>
              <TabsTrigger
                value="devices"
                className="h-14 rounded-none border-b-2 border-transparent data-[state=active]:border-[#1E3A8A] data-[state=active]:bg-white data-[state=active]:text-[#1E3A8A]"
              >
                Devices
              </TabsTrigger>
              <TabsTrigger
                value="network"
                className="h-14 rounded-none border-b-2 border-transparent data-[state=active]:border-[#1E3A8A] data-[state=active]:bg-white data-[state=active]:text-[#1E3A8A]"
              >
                Network
              </TabsTrigger>
            </TabsList>

            {/* General Settings */}
            <TabsContent value="general" className="flex-1 overflow-auto p-6">
              <div className="mx-auto max-w-2xl space-y-6">
                <h2 className="mb-6 text-[#1E3A8A]">General Settings</h2>

                <div className="space-y-2">
                  <Label htmlFor="language">Language</Label>
                  <Select
                    value={settingsLanguage}
                    onValueChange={(value: "EN" | "RW" | "FR") =>
                      setSettingsLanguage(value)
                    }
                  >
                    <SelectTrigger className="h-12 border-2">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="EN">English</SelectItem>
                      <SelectItem value="RW">Kinyarwanda</SelectItem>
                      <SelectItem value="FR">Français</SelectItem>
                    </SelectContent>
                  </Select>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="currency">Currency</Label>
                  <Select value={currency} onValueChange={setCurrency}>
                    <SelectTrigger className="h-12 border-2">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="RWF">RWF - Rwandan Franc</SelectItem>
                      <SelectItem value="USD">USD - US Dollar</SelectItem>
                      <SelectItem value="EUR">EUR - Euro</SelectItem>
                    </SelectContent>
                  </Select>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="timezone">Timezone</Label>
                  <Select value={timezone} onValueChange={setTimezone}>
                    <SelectTrigger className="h-12 border-2">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="Africa/Kigali">Africa/Kigali</SelectItem>
                      <SelectItem value="Africa/Nairobi">
                        Africa/Nairobi
                      </SelectItem>
                      <SelectItem value="Africa/Kampala">
                        Africa/Kampala
                      </SelectItem>
                    </SelectContent>
                  </Select>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="dateFormat">Date Format</Label>
                  <Select value={dateFormat} onValueChange={setDateFormat}>
                    <SelectTrigger className="h-12 border-2">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="DD/MM/YYYY">DD/MM/YYYY</SelectItem>
                      <SelectItem value="MM/DD/YYYY">MM/DD/YYYY</SelectItem>
                      <SelectItem value="YYYY-MM-DD">YYYY-MM-DD</SelectItem>
                    </SelectContent>
                  </Select>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="receiptHeader">Receipt Header</Label>
                  <Input
                    id="receiptHeader"
                    value={receiptHeader}
                    onChange={(e) => setReceiptHeader(e.target.value)}
                    className="h-12 border-2"
                  />
                </div>

                <div className="space-y-2">
                  <Label htmlFor="receiptFooter">Receipt Footer</Label>
                  <Input
                    id="receiptFooter"
                    value={receiptFooter}
                    onChange={(e) => setReceiptFooter(e.target.value)}
                    className="h-12 border-2"
                  />
                </div>

                <div className="flex gap-4 pt-6">
                  <Button
                    onClick={handleSaveSettings}
                    className="h-12 flex-1 bg-[#1E3A8A] hover:bg-[#1E40AF]"
                  >
                    Save Settings
                  </Button>
                  <Button
                    onClick={handleResetDefaults}
                    variant="outline"
                    className="h-12 flex-1 border-2"
                  >
                    Reset to Defaults
                  </Button>
                </div>
              </div>
            </TabsContent>

            {/* Price Management */}
            <TabsContent value="prices" className="flex-1 overflow-auto p-6">
              <div className="mx-auto max-w-4xl">
                <h2 className="mb-6 text-[#1E3A8A]">Price Management</h2>

                <div className="mb-6 overflow-hidden rounded-lg border-2 border-[#E5E7EB]">
                  <table className="w-full">
                    <thead className="bg-[#F9FAFB]">
                      <tr>
                        <th className="px-4 py-3 text-left text-[#6B7280]">
                          Service
                        </th>
                        <th className="px-4 py-3 text-left text-[#6B7280]">
                          Current Price
                        </th>
                        <th className="px-4 py-3 text-left text-[#6B7280]">
                          New Price
                        </th>
                        <th className="px-4 py-3 text-left text-[#6B7280]">
                          Effective Date
                        </th>
                      </tr>
                    </thead>
                    <tbody>
                      {prices.map((price, index) => (
                        <tr
                          key={index}
                          className="border-t border-[#E5E7EB] hover:bg-[#F9FAFB]"
                        >
                          <td className="px-4 py-3 text-[#111827]">
                            {price.service}
                          </td>
                          <td className="px-4 py-3 text-[#6B7280]">
                            {price.current.toLocaleString()} RWF
                          </td>
                          <td className="px-4 py-3">
                            <Input
                              type="number"
                              placeholder={price.current.toString()}
                              value={price.new}
                              onChange={(e) => {
                                const newPrices = [...prices];
                                newPrices[index].new = e.target.value;
                                setPrices(newPrices);
                              }}
                              className="h-10 w-32 border-2"
                            />
                          </td>
                          <td className="px-4 py-3">
                            <Input
                              type="date"
                              className="h-10 w-40 border-2"
                            />
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>

                <div className="flex gap-4">
                  <Button
                    onClick={handleSavePrices}
                    className="h-12 flex-1 bg-[#1E3A8A] hover:bg-[#1E40AF]"
                  >
                    Schedule Price Change
                  </Button>
                  <Button
                    variant="outline"
                    className="h-12 flex-1 border-2"
                  >
                    Apply to All Branches
                  </Button>
                </div>
              </div>
            </TabsContent>

            {/* Users Tab */}
            <TabsContent value="users" className="flex-1 overflow-auto p-6">
              <div className="mx-auto max-w-4xl">
                <div className="flex items-center justify-between mb-6">
                  <h2 className="text-[#1E3A8A]">User Management</h2>
                  <Button className="bg-[#1E3A8A] hover:bg-[#1E40AF]">
                    Add New User
                  </Button>
                </div>
                <p className="text-[#6B7280]">User management coming soon...</p>
              </div>
            </TabsContent>

            {/* Devices Tab */}
            <TabsContent value="devices" className="flex-1 overflow-auto p-6">
              <div className="mx-auto max-w-4xl">
                <h2 className="mb-6 text-[#1E3A8A]">Device Management</h2>
                <p className="text-[#6B7280]">Device management coming soon...</p>
              </div>
            </TabsContent>

            {/* Network Tab */}
            <TabsContent value="network" className="flex-1 overflow-auto p-6">
              <div className="mx-auto max-w-4xl">
                <h2 className="mb-6 text-[#1E3A8A]">Network Settings</h2>
                <p className="text-[#6B7280]">Network settings coming soon...</p>
              </div>
            </TabsContent>
          </Tabs>
        </div>
      </div>

      <Footer
        currentScreen="settings"
        onNavigate={onNavigate}
        onLogout={onLogout}
      />
    </div>
  );
}
