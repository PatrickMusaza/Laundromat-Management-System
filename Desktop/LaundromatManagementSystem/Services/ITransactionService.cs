using LaundromatManagementSystem.Models;
using LaundromatManagementSystem.ViewModels;

namespace LaundromatManagementSystem.Services
{
    public interface ITransactionService
    {
        Task<string> CreatePendingTransactionAsync(PaymentModalViewModel paymentViewModel, List<CartItem> cartItems);
        Task<bool> CompleteTransactionAsync(string transactionId, PaymentResult paymentResult);
        Task<List<TransactionModel>> GetPendingTransactionsAsync();
        Task<List<TransactionModel>> GetTransactionsByDateAsync(DateTime date);
        Task<TransactionModel?> GetTransactionByIdAsync(string transactionId);
        Task<bool> CancelTransactionAsync(string transactionId);
    }
}