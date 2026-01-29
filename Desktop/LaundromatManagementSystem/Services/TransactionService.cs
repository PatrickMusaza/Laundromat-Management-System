using LaundromatManagementSystem.Models;
using LaundromatManagementSystem.Repositories;
using LaundromatManagementSystem.ViewModels;

namespace LaundromatManagementSystem.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly ApplicationStateService _stateService;

        public TransactionService(
            ITransactionRepository transactionRepository,
            IServiceRepository serviceRepository,
            ApplicationStateService stateService)
        {
            _transactionRepository = transactionRepository;
            _serviceRepository = serviceRepository;
            _stateService = stateService;
        }

        public async Task<string> CreatePendingTransactionAsync(PaymentModalViewModel paymentViewModel)
        {
            try
            {
                // Generate transaction ID
                var transactionId = _stateService.GenerateTransactionId();

                // Convert cart items to transaction items
                var transactionItems = new List<TransactionItemModel>();
                foreach (var cartItem in paymentViewModel.CartItems)
                {
                    // Get service details from repository if available
                    if (int.TryParse(cartItem.ServiceId, out int serviceId))
                    {
                        var service = await _serviceRepository.GetServiceByIdAsync(serviceId);
                        if (service != null)
                        {
                            transactionItems.Add(new TransactionItemModel
                            {
                                ServiceId = service.Id,
                                ServiceName = service.Name,
                                ServiceDescription = GetTranslatedDescription(service, paymentViewModel.Language),
                                UnitPrice = (double)service.Price,
                                Quantity = cartItem.Quantity,
                                TotalPrice = (double)service.Price * cartItem.Quantity,
                                ServiceType = service.Type,
                                ServiceIcon = service.Icon
                            });
                        }
                        else
                        {
                            // Fallback to cart item data
                            transactionItems.Add(new TransactionItemModel
                            {
                                ServiceId = serviceId,
                                ServiceName = cartItem.Name,
                                UnitPrice = (double)cartItem.Price,
                                Quantity = cartItem.Quantity,
                                TotalPrice = (double)cartItem.TotalPrice,
                                ServiceType = cartItem.ServiceType ?? "unknown"
                            });
                        }
                    }
                }

                // Parse cash received amount
                double? cashReceived = null;
                if (paymentViewModel.SelectedMethod == "Cash" && !string.IsNullOrEmpty(paymentViewModel.CashReceived))
                {
                    var cleanAmount = paymentViewModel.CashReceived.Replace(",", "").Replace(".", "");
                    if (double.TryParse(cleanAmount, out double amount))
                    {
                        cashReceived = amount;
                    }
                }

                // Create transaction model
                var transactionModel = new TransactionModel
                {
                    TransactionId = transactionId,
                    Status = "pending",
                    PaymentMethod = paymentViewModel.SelectedMethod ?? "cash",
                    Subtotal = (double)paymentViewModel.Subtotal,
                    TaxAmount = (double)paymentViewModel.Tax,
                    TotalAmount = (double)paymentViewModel.GrandTotal,
                    CashReceived = cashReceived,
                    ChangeAmount = paymentViewModel.SelectedMethod == "Cash" ? (double)paymentViewModel.Change : null,
                    CustomerName = string.IsNullOrEmpty(paymentViewModel.Customer) ?
                        "Walk-in Customer" : paymentViewModel.Customer,
                    CustomerTin = paymentViewModel.TinNumber ?? "",
                    CustomerPhone = paymentViewModel.Customer ?? "",
                    TransactionDate = DateTime.UtcNow,
                    Items = transactionItems
                };

                // Save to database
                await _transactionRepository.CreatePendingTransactionAsync(transactionModel);

                return transactionId;
            }
            catch (Exception ex)
            {
                // Log error and return a transaction ID anyway for consistency
                Console.WriteLine($"Error creating pending transaction: {ex.Message}");
                return _stateService.GenerateTransactionId();
            }
        }

        public async Task<bool> CompleteTransactionAsync(string transactionId, PaymentResult paymentResult)
        {
            try
            {
                var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);
                if (transaction == null)
                {
                    Console.WriteLine($"Transaction not found: {transactionId}");
                    return false;
                }

                // Update transaction with payment result
                await _transactionRepository.CompleteTransactionAsync(transaction.Id, paymentResult);

                // Clear the cart after successful transaction
                _stateService.ClearCart();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error completing transaction: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CancelTransactionAsync(string transactionId)
        {
            try
            {
                var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);
                if (transaction == null)
                {
                    return false;
                }

                // Only allow cancellation of pending transactions
                if (transaction.Status != "pending")
                {
                    return false;
                }

                await _transactionRepository.UpdateTransactionStatusAsync(transaction.Id, "cancelled");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cancelling transaction: {ex.Message}");
                return false;
            }
        }

        public async Task<TransactionModel?> GetTransactionByIdAsync(string transactionId)
        {
            try
            {
                var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);
                if (transaction == null)
                {
                    return null;
                }

                return TransactionModel.FromEntity(transaction);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting transaction: {ex.Message}");
                return null;
            }
        }

        public async Task<List<TransactionModel>> GetPendingTransactionsAsync()
        {
            try
            {
                var transactions = await _transactionRepository.GetPendingTransactionsAsync();
                return transactions.Select(TransactionModel.FromEntity).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting pending transactions: {ex.Message}");
                return new List<TransactionModel>();
            }
        }

        public async Task<List<TransactionModel>> GetTransactionsByDateAsync(DateTime date)
        {
            try
            {
                var startDate = date.Date;
                var endDate = date.Date.AddDays(1).AddTicks(-1);

                var transactions = await _transactionRepository.GetTransactionsByDateRangeAsync(startDate, endDate);
                return transactions.Select(TransactionModel.FromEntity).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting transactions by date: {ex.Message}");
                return new List<TransactionModel>();
            }
        }

        public async Task<List<TransactionModel>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var transactions = await _transactionRepository.GetTransactionsByDateRangeAsync(startDate, endDate);
                return transactions.Select(TransactionModel.FromEntity).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting transactions by date range: {ex.Message}");
                return new List<TransactionModel>();
            }
        }

        public async Task<double> GetDailySalesAsync(DateTime date)
        {
            try
            {
                var startDate = date.Date;
                var endDate = date.Date.AddDays(1).AddTicks(-1);

                var transactions = await _transactionRepository.GetTransactionsByDateRangeAsync(startDate, endDate);

                // Only include completed transactions
                var completedTransactions = transactions
                    .Where(t => t.Status == "completed")
                    .ToList();

                return completedTransactions.Sum(t => t.TotalAmount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating daily sales: {ex.Message}");
                return 0;
            }
        }

        public async Task<double> GetMonthlySalesAsync(int year, int month)
        {
            try
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddTicks(-1);

                var transactions = await _transactionRepository.GetTransactionsByDateRangeAsync(startDate, endDate);

                // Only include completed transactions
                var completedTransactions = transactions
                    .Where(t => t.Status == "completed")
                    .ToList();

                return completedTransactions.Sum(t => t.TotalAmount);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating monthly sales: {ex.Message}");
                return 0;
            }
        }

        public async Task<int> GetTransactionCountAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var allTransactions = await _transactionRepository.GetAllTransactionsAsync();

                // Filter by date range if provided
                var filteredTransactions = allTransactions
                    .Where(t =>
                        (!startDate.HasValue || t.TransactionDate >= startDate.Value) &&
                        (!endDate.HasValue || t.TransactionDate <= endDate.Value))
                    .ToList();

                return filteredTransactions.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting transaction count: {ex.Message}");
                return 0;
            }
        }

        public async Task<List<TransactionModel>> SearchTransactionsAsync(string searchTerm)
        {
            try
            {
                var allTransactions = await _transactionRepository.GetAllTransactionsAsync();

                var searchResults = allTransactions
                    .Where(t =>
                        t.TransactionId.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        t.CustomerName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        t.CustomerTin.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        t.CustomerPhone.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        t.Items.Any(i => i.ServiceName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)))
                    .Select(TransactionModel.FromEntity)
                    .ToList();

                return searchResults;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching transactions: {ex.Message}");
                return new List<TransactionModel>();
            }
        }

        private string? GetTranslatedDescription(Entities.Service service, Language language)
        {
            return language switch
            {
                Language.EN => service.DescriptionEn,
                Language.RW => service.DescriptionRw,
                Language.FR => service.DescriptionFr,
                _ => service.DescriptionEn
            };
        }
    }
}