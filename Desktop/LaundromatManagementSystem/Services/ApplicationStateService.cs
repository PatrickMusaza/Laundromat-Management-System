using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json;
using LaundromatManagementSystem.Repositories;
using LaundromatManagementSystem.ViewModels;
using LaundromatManagementSystem.Models;

namespace LaundromatManagementSystem.Services;

public class ApplicationStateService : INotifyPropertyChanged
{
    private static ApplicationStateService _instance;
    public static ApplicationStateService Instance => _instance ??= new ApplicationStateService();

    private ITransactionRepository _transactionRepository;
    private Theme _currentTheme = Theme.Light;
    private Language _currentLanguage = Language.EN;
    private ObservableCollection<CartItem> _cartItems = new();
    private bool _showPaymentModal;

    public event PropertyChangedEventHandler PropertyChanged;

    public ApplicationStateService() { }

    public void SetTransactionRepository(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public Theme CurrentTheme
    {
        get => _currentTheme;
        set
        {
            if (_currentTheme != value)
            {
                _currentTheme = value;
                OnPropertyChanged();
                ThemeChanged?.Invoke(this, value);
            }
        }
    }

    public Language CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (_currentLanguage != value)
            {
                _currentLanguage = value;
                OnPropertyChanged();
                LanguageChanged?.Invoke(this, value);
            }
        }
    }

    public ObservableCollection<CartItem> CartItems
    {
        get => _cartItems;
        set
        {
            _cartItems = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CartTotal));
            OnPropertyChanged(nameof(ItemCount));
        }
    }

    public double CartTotal => CartItems.Sum(item => item.TotalPrice);
    public int ItemCount => CartItems.Sum(item => item.Quantity);

    public event EventHandler<Theme> ThemeChanged;
    public event EventHandler<Language> LanguageChanged;
    public event EventHandler CartUpdated;

    public void ClearCart()
    {
        _cartItems.Clear();
        OnPropertyChanged(nameof(CartItems));
        OnPropertyChanged(nameof(CartTotal));
        OnPropertyChanged(nameof(ItemCount));
        CartUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void AddToCart(CartItem item)
    {
        var existingItem = CartItems.FirstOrDefault(i => i.ServiceId == item.ServiceId);
        if (existingItem != null)
        {
            existingItem.Quantity += 1;
        }
        else
        {
            var newItem = new CartItem
            {
                Id = Guid.NewGuid().ToString(),
                ServiceId = item.ServiceId,
                Name = item.Name,
                Price = item.Price,
                Quantity = 1
            };
            CartItems.Add(newItem);
        }

        OnPropertyChanged(nameof(CartItems));
        OnPropertyChanged(nameof(CartTotal));
        OnPropertyChanged(nameof(ItemCount));
        CartUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateQuantity(string itemId, int changeQuantity)
    {
        var item = CartItems.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            var newQuantity = item.Quantity + changeQuantity;
            if (newQuantity <= 0)
            {
                RemoveItem(item.Id);
            }
            else
            {
                item.Quantity = newQuantity;
                OnPropertyChanged(nameof(CartItems));
                OnPropertyChanged(nameof(CartTotal));
                OnPropertyChanged(nameof(ItemCount));
                CartUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void RemoveItem(string itemId)
    {
        var item = CartItems.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            CartItems.Remove(item);
            OnPropertyChanged(nameof(CartItems));
            OnPropertyChanged(nameof(CartTotal));
            OnPropertyChanged(nameof(ItemCount));
            CartUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool ShowPaymentModal
    {
        get => _showPaymentModal;
        set
        {
            if (_showPaymentModal != value)
            {
                _showPaymentModal = value;
                OnPropertyChanged();
            }
        }
    }

    public void RequestPayment()
    {
        ShowPaymentModal = true;
    }

    public void ClosePaymentModal()
    {
        ShowPaymentModal = false;
    }

    public string GenerateTransactionId()
    {
        var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        var random = new Random().Next(1000, 9999);
        return $"T-{timestamp.ToString()[^6..]}-{random}";
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public async Task<string> CreatePendingTransactionAsync(PaymentModalViewModel paymentViewModel)
    {
        try
        {
            var transactionId = GenerateTransactionId();
            var transactionItems = paymentViewModel.CartItems.Select(item => new TransactionItemModel
            {
                ServiceId = int.TryParse(item.ServiceId, out int serviceId) ? serviceId : 0,
                ServiceName = item.Name,
                UnitPrice = (double)item.Price,
                Quantity = item.Quantity,
                TotalPrice = (double)item.TotalPrice,
                ServiceType = item.ServiceType ?? "unknown",
                ServiceIcon = item.Icon ?? "🧺"
            }).ToList();

            var transactionModel = new TransactionModel
            {
                TransactionId = transactionId,
                Status = "pending",
                PaymentMethod = paymentViewModel.SelectedMethod ?? "cash",
                Subtotal = (double)paymentViewModel.Subtotal,
                TaxAmount = (double)paymentViewModel.Tax,
                TotalAmount = (double)paymentViewModel.GrandTotal,
                CashReceived = paymentViewModel.SelectedMethod == "Cash" ?
                    double.Parse(paymentViewModel.CashReceived?.Replace(",", "").Replace(".", "") ?? "0") : null,
                ChangeAmount = paymentViewModel.SelectedMethod == "Cash" ? (double)paymentViewModel.Change : null,
                CustomerName = string.IsNullOrEmpty(paymentViewModel.Customer) ? "Walk-in Customer" : paymentViewModel.Customer,
                CustomerTin = paymentViewModel.TinNumber ?? "",
                CustomerPhone = paymentViewModel.Customer ?? "",
                TransactionDate = DateTime.UtcNow,
                Items = transactionItems
            };

            if (_transactionRepository != null)
            {
                await _transactionRepository.CreatePendingTransactionAsync(transactionModel);
            }
            else
            {
                await SaveTransactionToLocalStorage(transactionModel);
            }

            return transactionId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating pending transaction: {ex.Message}");
            return GenerateTransactionId();
        }
    }

    public async Task<bool> CompleteTransactionAsync(string transactionId, PaymentResult paymentResult)
    {
        try
        {
            if (_transactionRepository != null)
            {
                var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);
                if (transaction != null)
                {
                    await _transactionRepository.CompleteTransactionAsync(transaction.Id, paymentResult);
                    return true;
                }
            }

            await UpdateLocalTransactionStatus(transactionId, "completed", paymentResult);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error completing transaction: {ex.Message}");
            return false;
        }
    }

    private async Task SaveTransactionToLocalStorage(TransactionModel transaction)
    {
        try
        {
            var localTransactions = await GetLocalTransactionsAsync();
            localTransactions.Add(transaction);
            var json = JsonSerializer.Serialize(localTransactions);
            await SecureStorage.Default.SetAsync("pending_transactions", json);
        }
        catch
        {
            // Ignore errors in local storage
        }
    }

    private async Task UpdateLocalTransactionStatus(string transactionId, string status, PaymentResult paymentResult)
    {
        try
        {
            var localTransactions = await GetLocalTransactionsAsync();
            var transaction = localTransactions.FirstOrDefault(t => t.TransactionId == transactionId);
            if (transaction != null)
            {
                transaction.Status = status;
                var json = JsonSerializer.Serialize(localTransactions);
                await SecureStorage.Default.SetAsync("pending_transactions", json);
            }
        }
        catch
        {
            // Ignore errors
        }
    }

    private async Task<List<TransactionModel>> GetLocalTransactionsAsync()
    {
        try
        {
            var json = await SecureStorage.Default.GetAsync("pending_transactions");
            if (!string.IsNullOrEmpty(json))
            {
                return JsonSerializer.Deserialize<List<TransactionModel>>(json) ?? new List<TransactionModel>();
            }
        }
        catch
        {
            // Ignore errors
        }
        return new List<TransactionModel>();
    }
}