// Mock API responses - Replace with actual SQLite/Tauri API calls

export interface User {
  id: number;
  username: string;
  fullName: string;
  email: string;
  role: "Admin" | "Manager" | "Cashier";
  isActive: boolean;
  createDate: string;
  updateDate?: string;
}

export interface ServiceCategory {
  Id: number;
  Type: string;
  Icon: string;
  Color: string;
  NameEn: string;
  NameRw: string;
  NameFr: string;
  SortOrder: number;
  IsActive: boolean;
  CreateDate: string;
  UpdateDate?: string;
  UpdatedBy?: string;
}

export interface Service {
  Id: number;
  Name: string;
  Type: string;
  Price: string;
  Icon: string;
  Color: string;
  IsAvailable: boolean;
  NameEn: string;
  NameRw: string;
  NameFr: string;
  DescriptionEn?: string;
  DescriptionRw?: string;
  DescriptionFr?: string;
  CreateDate: string;
  UpdateDate?: string;
  UpdatedBy?: string;
  ServiceCategoryId: number;
}

export interface Transaction {
  Id: number;
  TransactionId: string;
  Status: string;
  PaymentMethod: string;
  Subtotal: number;
  TaxAmount: number;
  TotalAmount: number;
  CashReceived?: number;
  ChangeAmount?: number;
  CustomerName: string;
  CustomerTin: string;
  CustomerPhone: string;
  TransactionDate: string;
  PaymentDate?: string;
  CompletionDate?: string;
  CreateDate: string;
  UpdateDate?: string;
  UpdatedBy?: string;
}

export interface TransactionItem {
  Id: number;
  TransactionId: number;
  ServiceId: number;
  ServiceName: string;
  ServiceDescription?: string;
  UnitPrice: number;
  Quantity: number;
  TotalPrice: number;
  ServiceType: string;
  ServiceIcon: string;
  CreateDate: string;
}

export interface PaymentRecord {
  Id: number;
  TransactionId: number;
  PaymentMethod: string;
  Status: string;
  Amount: number;
  ReferenceNumber?: string;
  PaymentDetails?: string;
  PaymentDate: string;
  CompletionDate?: string;
  CreateDate: string;
  UpdateDate?: string;
}

// Mock data
export const mockUsers: User[] = [
  {
    id: 1,
    username: "admin",
    fullName: "Admin User",
    email: "admin@laundromat.com",
    role: "Admin",
    isActive: true,
    createDate: new Date().toISOString(),
  },
  {
    id: 2,
    username: "manager",
    fullName: "Manager User",
    email: "manager@laundromat.com",
    role: "Manager",
    isActive: true,
    createDate: new Date().toISOString(),
  },
];

export const mockCategories: ServiceCategory[] = [
  {
    Id: 1,
    Type: "washing",
    Icon: "🧺",
    Color: "#3B82F6",
    NameEn: "WASH",
    NameRw: "KARABA",
    NameFr: "LAVER",
    SortOrder: 1,
    IsActive: true,
    CreateDate: "2026-01-29 22:19:34.5938141",
  },
  {
    Id: 2,
    Type: "drying",
    Icon: "☀️",
    Color: "#F59E0B",
    NameEn: "DRY",
    NameRw: "UMISHA",
    NameFr: "SÉCHER",
    SortOrder: 2,
    IsActive: true,
    CreateDate: "2026-01-29 22:19:34.5943655",
  },
  {
    Id: 3,
    Type: "ironing",
    Icon: "🔥",
    Color: "#EF4444",
    NameEn: "IRON",
    NameRw: "IRON",
    NameFr: "REPASSER",
    SortOrder: 3,
    IsActive: true,
    CreateDate: "2026-01-29 22:19:34.5943655",
  },
  {
    Id: 4,
    Type: "addon",
    Icon: "➕",
    Color: "#10B981",
    NameEn: "ADD-ON",
    NameRw: "ONGERAHO",
    NameFr: "SUPPLÉMENT",
    SortOrder: 4,
    IsActive: true,
    CreateDate: "2026-01-29 22:19:34.5943672",
  },
];

export const mockServices: Service[] = [
  {
    Id: 1,
    Name: "Hot Water Wash",
    Type: "washing",
    Price: "5000.0",
    Icon: "🔥",
    Color: "#FEE2E2",
    IsAvailable: true,
    NameEn: "Hot Water Wash",
    NameRw: "Karaba y'amazi ashyushye",
    NameFr: "Lavage à l'eau chaude",
    DescriptionEn: "Complete wash with hot water",
    DescriptionRw: "Karaba yuzuye hamwe n'amazi ashyushye",
    DescriptionFr: "Lavage complet avec de l'eau chaude",
    CreateDate: "2026-01-29 22:19:34.5962375",
    ServiceCategoryId: 1,
  },
  {
    Id: 2,
    Name: "Cold Water Wash",
    Type: "washing",
    Price: "3000.0",
    Icon: "💧",
    Color: "#DBEAFE",
    IsAvailable: true,
    NameEn: "Cold Water Wash",
    NameRw: "Karaba y'amazi konje",
    NameFr: "Lavage à l'eau froide",
    DescriptionEn: "Gentle wash with cold water",
    DescriptionRw: "Karaba buhoro hamwe n'amazi konje",
    DescriptionFr: "Lavage doux à l'eau froide",
    CreateDate: "2026-01-29 22:19:34.5979984",
    ServiceCategoryId: 1,
  },
];

// Mock transactions with items
export const mockTransactions: Transaction[] = Array.from({ length: 30 }, (_, i) => ({
  Id: i + 1,
  TransactionId: `TXN-${String(i + 1).padStart(6, "0")}`,
  Status: i % 5 === 0 ? "Pending" : "Completed",
  PaymentMethod: ["Cash", "MoMo", "Card"][i % 3] as string,
  Subtotal: 5000 + i * 1000,
  TaxAmount: (5000 + i * 1000) * 0.18,
  TotalAmount: (5000 + i * 1000) * 1.18,
  CustomerName: `Customer ${i + 1}`,
  CustomerTin: `TIN${String(i + 1).padStart(9, "0")}`,
  CustomerPhone: `078${String(i + 1).padStart(7, "0")}`,
  TransactionDate: new Date(2026, 0, 29 - i).toISOString(),
  CreateDate: new Date(2026, 0, 29 - i).toISOString(),
}));

// Mock API functions - Replace these with actual backend calls
export const api = {
  // Users
  getUsers: async (): Promise<User[]> => {
    return Promise.resolve(mockUsers);
  },
  createUser: async (user: Omit<User, "id" | "createDate">): Promise<User> => {
    const newUser = {
      ...user,
      id: mockUsers.length + 1,
      createDate: new Date().toISOString(),
    };
    mockUsers.push(newUser);
    return Promise.resolve(newUser);
  },
  updateUser: async (id: number, user: Partial<User>): Promise<User> => {
    const index = mockUsers.findIndex((u) => u.id === id);
    if (index !== -1) {
      mockUsers[index] = { ...mockUsers[index], ...user, updateDate: new Date().toISOString() };
    }
    return Promise.resolve(mockUsers[index]);
  },
  deleteUser: async (id: number): Promise<void> => {
    const index = mockUsers.findIndex((u) => u.id === id);
    if (index !== -1) {
      mockUsers.splice(index, 1);
    }
    return Promise.resolve();
  },
  
  // Service Categories
  getCategories: async (): Promise<ServiceCategory[]> => {
    return Promise.resolve(mockCategories);
  },
  createCategory: async (category: Omit<ServiceCategory, "Id" | "CreateDate">): Promise<ServiceCategory> => {
    const newCategory = {
      ...category,
      Id: mockCategories.length + 1,
      CreateDate: new Date().toISOString(),
    };
    mockCategories.push(newCategory);
    return Promise.resolve(newCategory);
  },
  updateCategory: async (id: number, category: Partial<ServiceCategory>): Promise<ServiceCategory> => {
    const index = mockCategories.findIndex((c) => c.Id === id);
    if (index !== -1) {
      mockCategories[index] = { ...mockCategories[index], ...category, UpdateDate: new Date().toISOString() };
    }
    return Promise.resolve(mockCategories[index]);
  },
  deleteCategory: async (id: number): Promise<void> => {
    const index = mockCategories.findIndex((c) => c.Id === id);
    if (index !== -1) {
      mockCategories.splice(index, 1);
    }
    return Promise.resolve();
  },

  // Services
  getServices: async (): Promise<Service[]> => {
    return Promise.resolve(mockServices);
  },
  createService: async (service: Omit<Service, "Id" | "CreateDate">): Promise<Service> => {
    const newService = {
      ...service,
      Id: mockServices.length + 1,
      CreateDate: new Date().toISOString(),
    };
    mockServices.push(newService);
    return Promise.resolve(newService);
  },
  updateService: async (id: number, service: Partial<Service>): Promise<Service> => {
    const index = mockServices.findIndex((s) => s.Id === id);
    if (index !== -1) {
      mockServices[index] = { ...mockServices[index], ...service, UpdateDate: new Date().toISOString() };
    }
    return Promise.resolve(mockServices[index]);
  },
  deleteService: async (id: number): Promise<void> => {
    const index = mockServices.findIndex((s) => s.Id === id);
    if (index !== -1) {
      mockServices.splice(index, 1);
    }
    return Promise.resolve();
  },

  // Transactions
  getTransactions: async (): Promise<Transaction[]> => {
    return Promise.resolve(mockTransactions);
  },
  getTransaction: async (id: number): Promise<Transaction | undefined> => {
    return Promise.resolve(mockTransactions.find((t) => t.Id === id));
  },
};
