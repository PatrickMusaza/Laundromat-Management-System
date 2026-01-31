// Service Category Types
export interface ServiceCategory {
    Id: number;
    Type: string;
    Icon: string | null;
    Color: string;
    NameEn: string;
    NameRw: string;
    NameFr: string;
    SortOrder: number;
    IsActive: number;
    CreateDate: string;
    UpdateDate: string | null;
    UpdatedBy: string | null;
}

export interface CreateServiceCategory {
    Type: string;
    Icon?: string;
    Color: string;
    NameEn: string;
    NameRw: string;
    NameFr: string;
    SortOrder: number;
    IsActive?: number;
    UpdatedBy?: string;
}

export interface UpdateServiceCategory extends Partial<CreateServiceCategory> {}

// Service Types
export interface Service {
    Id: number;
    Name: string;
    Type: string;
    Price: string;
    Icon: string;
    Color: string;
    IsAvailable: number;
    NameEn: string;
    NameRw: string;
    NameFr: string;
    DescriptionEn: string | null;
    DescriptionRw: string | null;
    DescriptionFr: string | null;
    CreateDate: string;
    UpdateDate: string | null;
    UpdatedBy: string | null;
    ServiceCategoryId: number;
}

export interface CreateService {
    Name: string;
    Type: string;
    Price: string;
    Icon: string;
    Color: string;
    IsAvailable?: number;
    NameEn: string;
    NameRw: string;
    NameFr: string;
    DescriptionEn?: string;
    DescriptionRw?: string;
    DescriptionFr?: string;
    UpdatedBy?: string;
    ServiceCategoryId: number;
}

export interface UpdateService extends Partial<CreateService> {}

// Transaction Types
export interface Transaction {
    Id: number;
    TransactionId: string;
    Status: string;
    PaymentMethod: string;
    Subtotal: number;
    TaxAmount: number;
    TotalAmount: number;
    CashReceived: number | null;
    ChangeAmount: number | null;
    CustomerName: string;
    CustomerTin: string;
    CustomerPhone: string;
    TransactionDate: string;
    PaymentDate: string | null;
    CompletionDate: string | null;
    CreateDate: string;
    UpdateDate: string | null;
    UpdatedBy: string | null;
}

export interface CreateTransaction {
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
    UpdatedBy?: string;
}

export interface UpdateTransaction extends Partial<CreateTransaction> {}

// Transaction Item Types
export interface TransactionItem {
    Id: number;
    TransactionId: number;
    ServiceId: number;
    ServiceName: string;
    ServiceDescription: string | null;
    UnitPrice: number;
    Quantity: number;
    TotalPrice: number;
    ServiceType: string;
    ServiceIcon: string;
    CreateDate: string;
}

export interface CreateTransactionItem {
    TransactionId: number;
    ServiceId: number;
    ServiceName: string;
    ServiceDescription?: string;
    UnitPrice: number;
    Quantity: number;
    TotalPrice: number;
    ServiceType: string;
    ServiceIcon: string;
}

// Payment Record Types
export interface PaymentRecord {
    Id: number;
    TransactionId: number;
    PaymentMethod: string;
    Status: string;
    Amount: number;
    ReferenceNumber: string | null;
    PaymentDetails: string | null;
    PaymentDate: string;
    CompletionDate: string | null;
    CreateDate: string;
    UpdateDate: string | null;
}

export interface CreatePaymentRecord {
    TransactionId: number;
    PaymentMethod: string;
    Status: string;
    Amount: number;
    ReferenceNumber?: string;
    PaymentDetails?: string;
    PaymentDate: string;
    CompletionDate?: string;
}

export interface UpdatePaymentRecord extends Partial<CreatePaymentRecord> {}

// API Response Types
export interface ApiResponse<T> {
    success: boolean;
    data?: T;
    error?: string;
    message?: string;
}

export interface PaginatedResponse<T> {
    success: boolean;
    data: T[];
    total: number;
    page: number;
    limit: number;
    totalPages: number;
}