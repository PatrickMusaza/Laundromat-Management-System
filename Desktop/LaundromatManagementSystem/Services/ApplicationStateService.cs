using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LaundromatManagementSystem.Repositories;
using LaundromatManagementSystem.ViewModels;
using LaundromatManagementSystem.Models;

namespace LaundromatManagementSystem.Services;

public class ApplicationStateService : INotifyPropertyChanged
{
    private static ApplicationStateService _instance;
    public static ApplicationStateService Instance => _instance ??= new ApplicationStateService();
    private readonly ITransactionRepository _transactionRepository;

    private Theme _currentTheme = Theme.Light;
    private Language _currentLanguage = Language.EN;
    private ObservableCollection<CartItem> _cartItems = new();

    private bool _showPaymentModal;

    public event PropertyChangedEventHandler PropertyChanged;

    public ApplicationStateService() { }

    public ApplicationStateService(ITransactionRepository transactionRepository)
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
                // Notify all subscribers
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
                // Notify all subscribers
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

    public decimal CartTotal => CartItems.Sum(item => item.TotalPrice);
    public int ItemCount => CartItems.Sum(item => item.Quantity);

    public event EventHandler<Theme> ThemeChanged;
    public event EventHandler<Language> LanguageChanged;
    public event EventHandler CartUpdated;

    // Add this public method to clear cart from external classes
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
                Quantity = 1,
                Addons = new ObservableCollection<ServiceAddon>(
                    item.Addons ?? new ObservableCollection<ServiceAddon>())
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

    public void NotifyCartItemChanged(CartItem item)
    {
        // This will trigger the INotifyPropertyChanged on the item
        var index = CartItems.IndexOf(item);
        if (index >= 0)
        {
            // Replacing the item at the same index triggers collection change
            CartItems[index] = item;

            // Also notify totals changed
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
        return $"T-{DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()[^6..]}";
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public async Task<string> CreatePendingTransactionAsync(PaymentModalViewModel paymentViewModel)
    {
        try
        {
            // Generate transaction ID
            var transactionId = GenerateTransactionId();

            // Convert cart items to transaction items
            var transactionItems = CartItems.Select(item => new TransactionItemModel
            {
                ServiceId = int.Parse(item.ServiceId),
                ServiceName = item.Name,
                UnitPrice = (double)item.Price,
                Quantity = item.Quantity,
                TotalPrice = (double)item.TotalPrice,
                ServiceType = item.ServiceType ?? "unknown",
            }).ToList();

            // Create transaction model
            var transactionModel = new TransactionModel
            {
                TransactionId = transactionId,
                Status = "pending",
                PaymentMethod = paymentViewModel.SelectedMethod ?? "cash",
                Subtotal = (double)paymentViewModel.Subtotal,
                TaxAmount = (double)paymentViewModel.Tax,
                TotalAmount = (double)paymentViewModel.GrandTotal,
                CashReceived = paymentViewModel.SelectedMethod == "Cash" ?
                    double.Parse(paymentViewModel.CashReceived?.Replace(",", "").Replace(".", "") ?? "0") :
                    null,
                ChangeAmount = paymentViewModel.SelectedMethod == "Cash" ? (double)paymentViewModel.Change : null,
                CustomerName = "Walk-in Customer",
                CustomerTin = paymentViewModel.TinNumber ?? "",
                CustomerPhone = paymentViewModel.Customer ?? "",
                TransactionDate = DateTime.UtcNow,
                Items = transactionItems
            };

            // Save to database if repository is available
            if (_transactionRepository != null)
            {
                await _transactionRepository.CreatePendingTransactionAsync(transactionModel);
            }
            else
            {
                // Fallback: store in memory or local storage
                await SaveTransactionToLocalStorage(transactionModel);
            }

            return transactionId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating pending transaction: {ex.Message}");
            return GenerateTransactionId(); // Still generate ID even if save fails
        }
    }

    public async Task<bool> CompleteTransactionAsync(string transactionId, PaymentResult paymentResult)
    {
        try
        {
            if (_transactionRepository != null)
            {
                // Get transaction from database
                var transaction = await _transactionRepository.GetTransactionByIdAsync(transactionId);
                if (transaction != null)
                {
                    await _transactionRepository.CompleteTransactionAsync(transaction.Id, paymentResult);
                    return true;
                }
            }

            // Fallback: update local storage
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
        // Save to local storage or file
        // Implementation depends on your storage strategy
        await Task.CompletedTask;
    }

    private async Task UpdateLocalTransactionStatus(string transactionId, string status, PaymentResult paymentResult)
    {
        // Update local storage
        await Task.CompletedTask;
    }

}