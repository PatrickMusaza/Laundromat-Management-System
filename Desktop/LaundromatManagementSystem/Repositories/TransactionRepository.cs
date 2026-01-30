using LaundromatManagementSystem.Data;
using LaundromatManagementSystem.Entities;
using LaundromatManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LaundromatManagementSystem.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction> CreatePendingTransactionAsync(TransactionModel transactionModel)
        {
            // Check if transaction ID already exists
            var existing = await _context.Transactions
                .FirstOrDefaultAsync(t => t.TransactionId == transactionModel.TransactionId);
            
            if (existing != null)
            {
                throw new InvalidOperationException($"Transaction with ID {transactionModel.TransactionId} already exists");
            }

            var transaction = new Transaction
            {
                TransactionId = transactionModel.TransactionId,
                Status = "pending",
                PaymentMethod = transactionModel.PaymentMethod,
                Subtotal = transactionModel.Subtotal,
                TaxAmount = transactionModel.TaxAmount,
                TotalAmount = transactionModel.TotalAmount,
                CashReceived = transactionModel.CashReceived,
                ChangeAmount = transactionModel.ChangeAmount,
                CustomerName = transactionModel.CustomerName,
                CustomerTin = transactionModel.CustomerTin,
                CustomerPhone = transactionModel.CustomerPhone,
                TransactionDate = DateTime.UtcNow,
                CreateDate = DateTime.UtcNow
            };

            // Add transaction items
            foreach (var itemModel in transactionModel.Items)
            {
                var transactionItem = new TransactionItem
                {
                    ServiceId = itemModel.ServiceId,
                    ServiceName = itemModel.ServiceName,
                    ServiceDescription = itemModel.ServiceDescription,
                    UnitPrice = itemModel.UnitPrice,
                    Quantity = itemModel.Quantity,
                    TotalPrice = itemModel.TotalPrice,
                    ServiceType = itemModel.ServiceType,
                    ServiceIcon = itemModel.ServiceIcon,
                    CreateDate = DateTime.UtcNow
                };
                transaction.Items.Add(transactionItem);
            }

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<Transaction?> GetTransactionByIdAsync(string transactionId)
        {
            return await _context.Transactions
                .Include(t => t.Items)
                .Include(t => t.Payments)
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }

        public async Task<Transaction?> GetTransactionByDbIdAsync(int id)
        {
            return await _context.Transactions
                .Include(t => t.Items)
                .Include(t => t.Payments)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Transaction>> GetPendingTransactionsAsync()
        {
            return await _context.Transactions
                .Where(t => t.Status == "pending")
                .Include(t => t.Items)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<List<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Transactions
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .Include(t => t.Items)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<Transaction> UpdateTransactionStatusAsync(int transactionId, string status)
        {
            var transaction = await GetTransactionByDbIdAsync(transactionId);
            if (transaction == null)
            {
                throw new KeyNotFoundException($"Transaction with ID {transactionId} not found");
            }

            transaction.Status = status;
            transaction.UpdateDate = DateTime.UtcNow;

            if (status == "completed")
            {
                transaction.CompletionDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<PaymentRecord> AddPaymentRecordAsync(int transactionId, PaymentRecordModel paymentRecord)
        {
            var transaction = await GetTransactionByDbIdAsync(transactionId);
            if (transaction == null)
            {
                throw new KeyNotFoundException($"Transaction with ID {transactionId} not found");
            }

            var payment = new PaymentRecord
            {
                TransactionId = transactionId,
                PaymentMethod = paymentRecord.PaymentMethod,
                Status = paymentRecord.Status,
                Amount = paymentRecord.Amount,
                ReferenceNumber = paymentRecord.ReferenceNumber,
                PaymentDetails = paymentRecord.PaymentDetails,
                PaymentDate = DateTime.UtcNow,
                CreateDate = DateTime.UtcNow
            };

            transaction.Payments.Add(payment);
            
            // Update transaction status based on payment status
            if (paymentRecord.Status == "completed")
            {
                transaction.Status = "completed";
                transaction.PaymentDate = DateTime.UtcNow;
                transaction.CompletionDate = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Transaction> CompleteTransactionAsync(int transactionId, PaymentResult paymentResult)
        {
            var transaction = await GetTransactionByDbIdAsync(transactionId);
            if (transaction == null)
            {
                throw new KeyNotFoundException($"Transaction with ID {transactionId} not found");
            }

            // Update transaction with payment result
            transaction.Status = "completed";
            transaction.PaymentMethod = paymentResult.PaymentMethod.ToString();
            transaction.PaymentDate = DateTime.UtcNow;
            transaction.CompletionDate = DateTime.UtcNow;
            transaction.UpdateDate = DateTime.UtcNow;

            // Add payment record
            var paymentRecord = new PaymentRecord
            {
                PaymentMethod = paymentResult.PaymentMethod.ToString(),
                Status = "completed",
                Amount =(double) paymentResult.Amount,
                ReferenceNumber = paymentResult.TransactionId,
                PaymentDetails = $"Customer: {paymentResult.Customer}, Method: {paymentResult.PaymentMethod}",
                PaymentDate = DateTime.UtcNow,
                CreateDate = DateTime.UtcNow
            };

            transaction.Payments.Add(paymentRecord);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<bool> DeleteTransactionAsync(int transactionId)
        {
            var transaction = await GetTransactionByDbIdAsync(transactionId);
            if (transaction == null)
                return false;

            // Only allow deletion of pending transactions
            if (transaction.Status != "pending")
                return false;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync(int page = 1, int pageSize = 50)
        {
            return await _context.Transactions
                .Include(t => t.Items)
                .Include(t => t.Payments)
                .OrderByDescending(t => t.TransactionDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}