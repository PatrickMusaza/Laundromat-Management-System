import { 
    ServiceCategory, CreateServiceCategory, UpdateServiceCategory,
    Service, CreateService, UpdateService,
    Transaction, CreateTransaction, UpdateTransaction,
    PaymentRecord, CreatePaymentRecord, UpdatePaymentRecord,
    ApiResponse
} from './types';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:3000/api';

class ApiClient {
    private async request<T>(endpoint: string, options: RequestInit = {}): Promise<ApiResponse<T>> {
        const url = `${API_BASE_URL}${endpoint}`;
        const defaultOptions: RequestInit = {
            headers: {
                'Content-Type': 'application/json',
            },
        };

        try {
            const response = await fetch(url, { ...defaultOptions, ...options });
            const data = await response.json();
            
            if (!response.ok) {
                throw new Error(data.error || `HTTP ${response.status}`);
            }
            
            return data;
        } catch (error) {
            console.error('API request failed:', error);
            throw error;
        }
    }

    // Service Categories
    async getServiceCategories(): Promise<ApiResponse<ServiceCategory[]>> {
        return this.request('/service-categories');
    }

    async getServiceCategory(id: number): Promise<ApiResponse<ServiceCategory>> {
        return this.request(`/service-categories/${id}`);
    }

    async createServiceCategory(data: CreateServiceCategory): Promise<ApiResponse<ServiceCategory>> {
        return this.request('/service-categories', {
            method: 'POST',
            body: JSON.stringify(data)
        });
    }

    async updateServiceCategory(id: number, data: UpdateServiceCategory): Promise<ApiResponse<ServiceCategory>> {
        return this.request(`/service-categories/${id}`, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
    }

    async deleteServiceCategory(id: number): Promise<ApiResponse<{ message: string }>> {
        return this.request(`/service-categories/${id}`, {
            method: 'DELETE'
        });
    }

    // Services
    async getServices(): Promise<ApiResponse<Service[]>> {
        return this.request('/services');
    }

    async getAvailableServices(): Promise<ApiResponse<Service[]>> {
        return this.request('/services/available');
    }

    async getService(id: number): Promise<ApiResponse<Service>> {
        return this.request(`/services/${id}`);
    }

    async createService(data: CreateService): Promise<ApiResponse<Service>> {
        return this.request('/services', {
            method: 'POST',
            body: JSON.stringify(data)
        });
    }

    async updateService(id: number, data: UpdateService): Promise<ApiResponse<Service>> {
        return this.request(`/services/${id}`, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
    }

    async deleteService(id: number): Promise<ApiResponse<{ message: string }>> {
        return this.request(`/services/${id}`, {
            method: 'DELETE'
        });
    }

    // Transactions
    async getTransactions(): Promise<ApiResponse<Transaction[]>> {
        return this.request('/transactions');
    }

    async getTransaction(id: number): Promise<ApiResponse<Transaction & { items: any[] }>> {
        return this.request(`/transactions/${id}`);
    }

    async createTransaction(data: CreateTransaction & { items: any[] }): Promise<ApiResponse<Transaction & { items: any[] }>> {
        return this.request('/transactions', {
            method: 'POST',
            body: JSON.stringify(data)
        });
    }

    async updateTransaction(id: number, data: UpdateTransaction): Promise<ApiResponse<Transaction>> {
        return this.request(`/transactions/${id}`, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
    }

    async deleteTransaction(id: number): Promise<ApiResponse<{ message: string }>> {
        return this.request(`/transactions/${id}`, {
            method: 'DELETE'
        });
    }

    // Payment Records
    async getPaymentRecords(): Promise<ApiResponse<PaymentRecord[]>> {
        return this.request('/payment-records');
    }

    async getPaymentRecord(id: number): Promise<ApiResponse<PaymentRecord>> {
        return this.request(`/payment-records/${id}`);
    }

    async createPaymentRecord(data: CreatePaymentRecord): Promise<ApiResponse<PaymentRecord>> {
        return this.request('/payment-records', {
            method: 'POST',
            body: JSON.stringify(data)
        });
    }

    async updatePaymentRecord(id: number, data: UpdatePaymentRecord): Promise<ApiResponse<PaymentRecord>> {
        return this.request(`/payment-records/${id}`, {
            method: 'PUT',
            body: JSON.stringify(data)
        });
    }

    async deletePaymentRecord(id: number): Promise<ApiResponse<{ message: string }>> {
        return this.request(`/payment-records/${id}`, {
            method: 'DELETE'
        });
    }
}

export const apiClient = new ApiClient();