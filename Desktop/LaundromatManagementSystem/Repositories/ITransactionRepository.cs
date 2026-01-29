using LaundromatManagementSystem.Entities;
using LaundromatManagementSystem.Models;

namespace LaundromatManagementSystem.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction> CreatePendingTransactionAsync(TransactionModel transactionModel);
        Task<Transaction?> GetTransactionByIdAsync(string transactionId);
        Task<Transaction?> GetTransactionByDbIdAsync(int id);
        Task<List<Transaction>> GetPendingTransactionsAsync();
        Task<List<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Transaction> UpdateTransactionStatusAsync(int transactionId, string status);
        Task<PaymentRecord> AddPaymentRecordAsync(int transactionId, PaymentRecordModel paymentRecord);
        Task<Transaction> CompleteTransactionAsync(int transactionId, PaymentResult paymentResult);
        Task<bool> DeleteTransactionAsync(int transactionId);
        Task<List<Transaction>> GetAllTransactionsAsync(int page = 1, int pageSize = 50);
    }
}