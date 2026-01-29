import { useState, useEffect } from "react";
import { Plus, RefreshCw, Edit, Trash2, FileDown, FileUp } from "lucide-react";
import { Button } from "@/app/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/app/components/ui/card";
import { Input } from "@/app/components/ui/input";
import { Label } from "@/app/components/ui/label";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/app/components/ui/dialog";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/app/components/ui/select";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/app/components/ui/tabs";
import { api, ServiceCategory, Service } from "@/app/lib/api";
import { toast } from "sonner";

export default function ServicesPage() {
  const [categories, setCategories] = useState<ServiceCategory[]>([]);
  const [services, setServices] = useState<Service[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<ServiceCategory | null>(null);
  const [showCategoryDialog, setShowCategoryDialog] = useState(false);
  const [showServiceDialog, setShowServiceDialog] = useState(false);
  const [editingCategory, setEditingCategory] = useState<ServiceCategory | null>(null);
  const [editingService, setEditingService] = useState<Service | null>(null);
  const [activeTab, setActiveTab] = useState("categories");

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    try {
      const [categoriesData, servicesData] = await Promise.all([
        api.getCategories(),
        api.getServices(),
      ]);
      setCategories(categoriesData);
      setServices(servicesData);
    } catch (error) {
      toast.error("Failed to load data");
    }
  };

  const handleDeleteCategory = async (id: number) => {
    if (confirm("Are you sure you want to delete this category?")) {
      try {
        await api.deleteCategory(id);
        await loadData();
        toast.success("Category deleted successfully");
      } catch (error) {
        toast.error("Failed to delete category");
      }
    }
  };

  const handleDeleteService = async (id: number) => {
    if (confirm("Are you sure you want to delete this service?")) {
      try {
        await api.deleteService(id);
        await loadData();
        toast.success("Service deleted successfully");
      } catch (error) {
        toast.error("Failed to delete service");
      }
    }
  };

  const filteredServices = selectedCategory
    ? services.filter((s) => s.ServiceCategoryId === selectedCategory.Id)
    : services;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="mb-2">Services Management</h1>
          <p className="text-gray-600">Manage service categories and services</p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={loadData}>
            <RefreshCw className="mr-2 h-4 w-4" />
            Refresh
          </Button>
          <Button variant="outline">
            <FileDown className="mr-2 h-4 w-4" />
            Export
          </Button>
          <Button variant="outline">
            <FileUp className="mr-2 h-4 w-4" />
            Import
          </Button>
        </div>
      </div>

      <div className="grid gap-6 md:grid-cols-[40%_60%]">
        {/* Left: Categories */}
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle>Categories</CardTitle>
            <Button
              size="sm"
              onClick={() => {
                setEditingCategory(null);
                setShowCategoryDialog(true);
              }}
            >
              <Plus className="mr-2 h-4 w-4" />
              Create
            </Button>
          </CardHeader>
          <CardContent>
            <div className="space-y-2">
              {categories.map((category) => (
                <div
                  key={category.Id}
                  onClick={() => setSelectedCategory(category)}
                  className={`flex cursor-pointer items-center justify-between rounded-lg border p-4 transition-colors hover:bg-gray-50 ${
                    selectedCategory?.Id === category.Id ? "border-blue-500 bg-blue-50" : ""
                  }`}
                >
                  <div className="flex items-center gap-3">
                    <span className="text-2xl">{category.Icon}</span>
                    <div>
                      <h3 className="font-semibold">{category.NameEn}</h3>
                      <p className="text-sm text-gray-500">{category.Type}</p>
                    </div>
                  </div>
                  <div className="flex gap-2">
                    <Button
                      size="sm"
                      variant="ghost"
                      onClick={(e) => {
                        e.stopPropagation();
                        setEditingCategory(category);
                        setShowCategoryDialog(true);
                      }}
                    >
                      <Edit className="h-4 w-4" />
                    </Button>
                    <Button
                      size="sm"
                      variant="ghost"
                      onClick={(e) => {
                        e.stopPropagation();
                        handleDeleteCategory(category.Id);
                      }}
                    >
                      <Trash2 className="h-4 w-4 text-red-500" />
                    </Button>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Right: Services */}
        <Card>
          <CardHeader className="flex flex-row items-center justify-between">
            <CardTitle>
              Services {selectedCategory && `- ${selectedCategory.NameEn}`}
            </CardTitle>
            <Button
              size="sm"
              onClick={() => {
                setEditingService(null);
                setShowServiceDialog(true);
              }}
              disabled={!selectedCategory}
            >
              <Plus className="mr-2 h-4 w-4" />
              Create
            </Button>
          </CardHeader>
          <CardContent>
            {!selectedCategory ? (
              <div className="py-12 text-center text-gray-500">
                Select a category to view services
              </div>
            ) : (
              <div className="space-y-2">
                {filteredServices.length === 0 ? (
                  <div className="py-12 text-center text-gray-500">
                    No services found for this category
                  </div>
                ) : (
                  filteredServices.map((service) => (
                    <div
                      key={service.Id}
                      className="flex items-center justify-between rounded-lg border p-4 hover:bg-gray-50"
                    >
                      <div className="flex items-center gap-3">
                        <span className="text-2xl">{service.Icon}</span>
                        <div>
                          <h3 className="font-semibold">{service.NameEn}</h3>
                          <p className="text-sm text-gray-500">{service.DescriptionEn}</p>
                          <p className="mt-1 font-semibold text-blue-600">
                            RWF {parseFloat(service.Price).toLocaleString()}
                          </p>
                        </div>
                      </div>
                      <div className="flex gap-2">
                        <span
                          className={`rounded-full px-3 py-1 text-xs ${
                            service.IsAvailable
                              ? "bg-green-100 text-green-800"
                              : "bg-red-100 text-red-800"
                          }`}
                        >
                          {service.IsAvailable ? "Available" : "Unavailable"}
                        </span>
                        <Button
                          size="sm"
                          variant="ghost"
                          onClick={() => {
                            setEditingService(service);
                            setShowServiceDialog(true);
                          }}
                        >
                          <Edit className="h-4 w-4" />
                        </Button>
                        <Button
                          size="sm"
                          variant="ghost"
                          onClick={() => handleDeleteService(service.Id)}
                        >
                          <Trash2 className="h-4 w-4 text-red-500" />
                        </Button>
                      </div>
                    </div>
                  ))
                )}
              </div>
            )}
          </CardContent>
        </Card>
      </div>

      {/* Category Dialog */}
      <CategoryDialog
        open={showCategoryDialog}
        onClose={() => setShowCategoryDialog(false)}
        category={editingCategory}
        onSave={loadData}
      />

      {/* Service Dialog */}
      <ServiceDialog
        open={showServiceDialog}
        onClose={() => setShowServiceDialog(false)}
        service={editingService}
        categories={categories}
        selectedCategoryId={selectedCategory?.Id}
        onSave={loadData}
      />
    </div>
  );
}

// Category Dialog Component
function CategoryDialog({
  open,
  onClose,
  category,
  onSave,
}: {
  open: boolean;
  onClose: () => void;
  category: ServiceCategory | null;
  onSave: () => void;
}) {
  const [formData, setFormData] = useState({
    Type: "",
    Icon: "",
    Color: "#3B82F6",
    NameEn: "",
    NameRw: "",
    NameFr: "",
    SortOrder: 1,
    IsActive: true,
  });

  useEffect(() => {
    if (category) {
      setFormData({
        Type: category.Type,
        Icon: category.Icon,
        Color: category.Color,
        NameEn: category.NameEn,
        NameRw: category.NameRw,
        NameFr: category.NameFr,
        SortOrder: category.SortOrder,
        IsActive: category.IsActive,
      });
    } else {
      setFormData({
        Type: "",
        Icon: "",
        Color: "#3B82F6",
        NameEn: "",
        NameRw: "",
        NameFr: "",
        SortOrder: 1,
        IsActive: true,
      });
    }
  }, [category, open]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (category) {
        await api.updateCategory(category.Id, formData);
        toast.success("Category updated successfully");
      } else {
        await api.createCategory(formData);
        toast.success("Category created successfully");
      }
      onSave();
      onClose();
    } catch (error) {
      toast.error("Failed to save category");
    }
  };

  return (
    <Dialog open={open} onOpenChange={(isOpen) => !isOpen && onClose()}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle>{category ? "Edit Category" : "Create Category"}</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <div className="grid gap-4 md:grid-cols-2">
            <div>
              <Label>Type</Label>
              <Input
                value={formData.Type}
                onChange={(e) => setFormData({ ...formData, Type: e.target.value })}
                required
              />
            </div>
            <div>
              <Label>Icon (Emoji)</Label>
              <Input
                value={formData.Icon}
                onChange={(e) => setFormData({ ...formData, Icon: e.target.value })}
                required
              />
            </div>
            <div>
              <Label>Color</Label>
              <Input
                type="color"
                value={formData.Color}
                onChange={(e) => setFormData({ ...formData, Color: e.target.value })}
                required
              />
            </div>
            <div>
              <Label>Sort Order</Label>
              <Input
                type="number"
                value={formData.SortOrder}
                onChange={(e) =>
                  setFormData({ ...formData, SortOrder: parseInt(e.target.value) })
                }
                required
              />
            </div>
            <div>
              <Label>Name (English)</Label>
              <Input
                value={formData.NameEn}
                onChange={(e) => setFormData({ ...formData, NameEn: e.target.value })}
                required
              />
            </div>
            <div>
              <Label>Name (Kinyarwanda)</Label>
              <Input
                value={formData.NameRw}
                onChange={(e) => setFormData({ ...formData, NameRw: e.target.value })}
                required
              />
            </div>
            <div>
              <Label>Name (French)</Label>
              <Input
                value={formData.NameFr}
                onChange={(e) => setFormData({ ...formData, NameFr: e.target.value })}
                required
              />
            </div>
          </div>
          <DialogFooter>
            <Button type="button" variant="outline" onClick={onClose}>
              Cancel
            </Button>
            <Button type="submit">Save</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}

// Service Dialog Component
function ServiceDialog({
  open,
  onClose,
  service,
  categories,
  selectedCategoryId,
  onSave,
}: {
  open: boolean;
  onClose: () => void;
  service: Service | null;
  categories: ServiceCategory[];
  selectedCategoryId?: number;
  onSave: () => void;
}) {
  const [formData, setFormData] = useState({
    Name: "",
    Type: "",
    Price: "0",
    Icon: "",
    Color: "#DBEAFE",
    IsAvailable: true,
    NameEn: "",
    NameRw: "",
    NameFr: "",
    DescriptionEn: "",
    DescriptionRw: "",
    DescriptionFr: "",
    ServiceCategoryId: selectedCategoryId || 0,
  });

  useEffect(() => {
    if (service) {
      setFormData({
        Name: service.Name,
        Type: service.Type,
        Price: service.Price,
        Icon: service.Icon,
        Color: service.Color,
        IsAvailable: service.IsAvailable,
        NameEn: service.NameEn,
        NameRw: service.NameRw,
        NameFr: service.NameFr,
        DescriptionEn: service.DescriptionEn || "",
        DescriptionRw: service.DescriptionRw || "",
        DescriptionFr: service.DescriptionFr || "",
        ServiceCategoryId: service.ServiceCategoryId,
      });
    } else if (selectedCategoryId) {
      setFormData({
        ...formData,
        ServiceCategoryId: selectedCategoryId,
      });
    }
  }, [service, selectedCategoryId, open]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (service) {
        await api.updateService(service.Id, formData);
        toast.success("Service updated successfully");
      } else {
        await api.createService(formData);
        toast.success("Service created successfully");
      }
      onSave();
      onClose();
    } catch (error) {
      toast.error("Failed to save service");
    }
  };

  return (
    <Dialog open={open} onOpenChange={(isOpen) => !isOpen && onClose()}>
      <DialogContent className="max-w-3xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>{service ? "Edit Service" : "Create Service"}</DialogTitle>
        </DialogHeader>
        <form onSubmit={handleSubmit} className="space-y-4">
          <Tabs defaultValue="basic">
            <TabsList className="grid w-full grid-cols-3">
              <TabsTrigger value="basic">Basic Info</TabsTrigger>
              <TabsTrigger value="translations">Translations</TabsTrigger>
              <TabsTrigger value="descriptions">Descriptions</TabsTrigger>
            </TabsList>
            
            <TabsContent value="basic" className="space-y-4">
              <div className="grid gap-4 md:grid-cols-2">
                <div>
                  <Label>Category</Label>
                  <Select
                    value={formData.ServiceCategoryId.toString()}
                    onValueChange={(value) =>
                      setFormData({ ...formData, ServiceCategoryId: parseInt(value) })
                    }
                  >
                    <SelectTrigger>
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      {categories.map((cat) => (
                        <SelectItem key={cat.Id} value={cat.Id.toString()}>
                          {cat.Icon} {cat.NameEn}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
                <div>
                  <Label>Price (RWF)</Label>
                  <Input
                    type="number"
                    step="0.01"
                    value={formData.Price}
                    onChange={(e) => setFormData({ ...formData, Price: e.target.value })}
                    required
                  />
                </div>
                <div>
                  <Label>Icon (Emoji)</Label>
                  <Input
                    value={formData.Icon}
                    onChange={(e) => setFormData({ ...formData, Icon: e.target.value })}
                    required
                  />
                </div>
                <div>
                  <Label>Color</Label>
                  <Input
                    type="color"
                    value={formData.Color}
                    onChange={(e) => setFormData({ ...formData, Color: e.target.value })}
                  />
                </div>
                <div className="flex items-center gap-2">
                  <input
                    type="checkbox"
                    checked={formData.IsAvailable}
                    onChange={(e) =>
                      setFormData({ ...formData, IsAvailable: e.target.checked })
                    }
                    className="h-4 w-4"
                  />
                  <Label>Available</Label>
                </div>
              </div>
            </TabsContent>

            <TabsContent value="translations" className="space-y-4">
              <div className="space-y-4">
                <div>
                  <Label>Name (English)</Label>
                  <Input
                    value={formData.NameEn}
                    onChange={(e) => setFormData({ ...formData, NameEn: e.target.value })}
                    required
                  />
                </div>
                <div>
                  <Label>Name (Kinyarwanda)</Label>
                  <Input
                    value={formData.NameRw}
                    onChange={(e) => setFormData({ ...formData, NameRw: e.target.value })}
                    required
                  />
                </div>
                <div>
                  <Label>Name (French)</Label>
                  <Input
                    value={formData.NameFr}
                    onChange={(e) => setFormData({ ...formData, NameFr: e.target.value })}
                    required
                  />
                </div>
              </div>
            </TabsContent>

            <TabsContent value="descriptions" className="space-y-4">
              <div className="space-y-4">
                <div>
                  <Label>Description (English)</Label>
                  <Input
                    value={formData.DescriptionEn}
                    onChange={(e) =>
                      setFormData({ ...formData, DescriptionEn: e.target.value })
                    }
                  />
                </div>
                <div>
                  <Label>Description (Kinyarwanda)</Label>
                  <Input
                    value={formData.DescriptionRw}
                    onChange={(e) =>
                      setFormData({ ...formData, DescriptionRw: e.target.value })
                    }
                  />
                </div>
                <div>
                  <Label>Description (French)</Label>
                  <Input
                    value={formData.DescriptionFr}
                    onChange={(e) =>
                      setFormData({ ...formData, DescriptionFr: e.target.value })
                    }
                  />
                </div>
              </div>
            </TabsContent>
          </Tabs>

          <DialogFooter>
            <Button type="button" variant="outline" onClick={onClose}>
              Cancel
            </Button>
            <Button type="submit">Save</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
