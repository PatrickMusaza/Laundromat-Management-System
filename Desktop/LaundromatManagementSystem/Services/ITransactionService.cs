using LaundromatManagementSystem.Models;
using LaundromatManagementSystem.ViewModels;

namespace LaundromatManagementSystem.Services
{
    public interface ITransactionService
    {
        Task<string> CreatePendingTransactionAsync(PaymentModalViewModel paymentViewModel);
        Task<bool> CompleteTransactionAsync(string transactionId, PaymentResult paymentResult);
        Task<bool> CancelTransactionAsync(string transactionId);
        Task<TransactionModel?> GetTransactionByIdAsync(string transactionId);
        Task<List<TransactionModel>> GetPendingTransactionsAsync();
        Task<List<TransactionModel>> GetTransactionsByDateAsync(DateTime date);
        Task<List<TransactionModel>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<double> GetDailySalesAsync(DateTime date);
        Task<double> GetMonthlySalesAsync(int year, int month);
        Task<int> GetTransactionCountAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<List<TransactionModel>> SearchTransactionsAsync(string searchTerm);
    }
}