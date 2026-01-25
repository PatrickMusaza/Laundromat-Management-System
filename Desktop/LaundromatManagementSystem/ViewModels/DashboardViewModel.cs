using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaundromatManagementSystem.Models;
using LaundromatManagementSystem.Services;
using System.Collections.ObjectModel;

namespace LaundromatManagementSystem.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        // Using the singleton ApplicationStateService
        private readonly ApplicationStateService _stateService = ApplicationStateService.Instance;

        [ObservableProperty]
        private Language _language;

        [ObservableProperty]
        private Theme _theme;

        [ObservableProperty]
        private string _selectedCategory = "washing";

        [ObservableProperty]
        private ObservableCollection<CartItem> _cart = new();

        [ObservableProperty]
        private decimal _subtotal;

        [ObservableProperty]
        private decimal _tax;

        [ObservableProperty]
        private decimal _total;

        [ObservableProperty]
        private bool _showPaymentModal;

        [ObservableProperty]
        private string _transactionId = string.Empty;

        private readonly ICartService _cartService;
        private readonly IServiceService _serviceService;

        [ObservableProperty]
        public Color _dashboardBackgroundColor;

        public DashboardViewModel(ICartService cartService, IServiceService serviceService)
        {
            _cartService = cartService;
            _serviceService = serviceService;

            // Initialize from state service
            _language = _stateService.CurrentLanguage;
            _theme = _stateService.CurrentTheme;
            Cart = new ObservableCollection<CartItem>(_stateService.CartItems);
            CalculateTotals();

            DashboardBackgroundColor = Colors.White;

            // Subscribe to state changes
            _stateService.PropertyChanged += OnStateChanged;
            _stateService.CartUpdated += OnCartUpdated;

            // Subscribe to cart service events (if still needed)
            _cartService.CartUpdated += OnServiceCartUpdated;
        }

        // Override the setters to update the state service
        partial void OnLanguageChanged(Language value)
        {
            _stateService.CurrentLanguage = value;
        }

        partial void OnThemeChanged(Theme value)
        {
            _stateService.CurrentTheme = value;
        }

        // Listen for state changes from other views/components
        private void OnStateChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                switch (e.PropertyName)
                {
                    case nameof(_stateService.CurrentLanguage):
                        if (Language != _stateService.CurrentLanguage)
                            Language = _stateService.CurrentLanguage;
                        break;

                    case nameof(_stateService.CurrentTheme):
                        if (Theme != _stateService.CurrentTheme)
                            Theme = _stateService.CurrentTheme;
                        DashboardBackgroundColor = GetDashboardBackgroundColor();
                        break;

                    case nameof(_stateService.CartItems):
                        if (!Cart.SequenceEqual(_stateService.CartItems))
                        {
                            Cart = new ObservableCollection<CartItem>(_stateService.CartItems);
                            CalculateTotals();
                        }
                        break;
                }
            });
        }

        [RelayCommand]
        private void ChangeCategory(string category) => SelectedCategory = category;

        [RelayCommand]
        private void AddToCart(CartItem item)
        {
            // Use state service to add item
            _stateService.AddToCart(item);

            // Also update the cart service if needed
            var cartItem = new CartItem
            {
                Id = $"{item.Name}-{Guid.NewGuid():N}",
                Name = item.Name,
                Price = item.Price,
                Addons = new ObservableCollection<ServiceAddon>(item.Addons),
                Quantity = 1
            };

            _cartService.AddItem(cartItem);
        }

        [RelayCommand]
        private void RemoveFromCart(string itemId)
        {
            // Remove via state service
            _stateService.RemoveItem(itemId);

            // Also update the cart service if needed
            _cartService.RemoveItem(itemId);

            // Force UI update
            OnPropertyChanged(nameof(Cart));
            CalculateTotals();
        }

        [RelayCommand]
        private void UpdateQuantity((string itemId, int quantity) parameters)
        {

            if (parameters.quantity <= 0)
            {
                RemoveFromCart(parameters.itemId);
                return;
            }

            // Find the item by Id (not ServiceId)
            var item = Cart.FirstOrDefault(i => i.Id == parameters.itemId);
            if (item != null)
            {
                // Update via state service
                _stateService.UpdateQuantity(item.ServiceId, parameters.quantity);

                // Also update the cart service if needed
                _cartService.UpdateQuantity(parameters.itemId, parameters.quantity);

                CalculateTotals();
            }
        }

        [RelayCommand]
        private void ProcessPayment()
        {
            if (Cart.Count == 0) return;

            ShowPaymentModal = true;
            TransactionId = $"T-{DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()[^6..]}";
        }

        [RelayCommand]
        private void ClosePaymentModal() => ShowPaymentModal = false;

        [RelayCommand]
        private void CompletePayment((PaymentMethod paymentMethod, string customer) parameters)
        {
            var transaction = new Transaction
            {
                Id = TransactionId,
                Timestamp = DateTime.UtcNow,
                Amount = Total,
                PaymentMethod = parameters.paymentMethod,
                Items = new ObservableCollection<CartItem>(Cart),
                Customer = parameters.customer
            };

            // TODO: Add transaction to history

            // Clear all carts
            _stateService.CartItems.Clear();
            _cartService.ClearCart();

            Cart.Clear();
            CalculateTotals();
            ShowPaymentModal = false;
        }

        private void OnServiceCartUpdated(object? sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                // Sync from cart service to state service
                var serviceItems = _cartService.GetCartItems();
                if (!serviceItems.SequenceEqual(_stateService.CartItems))
                {
                    _stateService.CartItems = new ObservableCollection<CartItem>(serviceItems);
                }
            });
        }

        private void OnCartUpdated(object? sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                CalculateTotals();
            });
        }

        private void CalculateTotals()
        {
            Subtotal = Cart.Sum(item => item.TotalPrice);
            Tax = Math.Round(Subtotal * 0.1m, 2);
            Total = Subtotal + Tax;
        }

        // Clean up subscriptions
        public void Cleanup()
        {
            _stateService.PropertyChanged -= OnStateChanged;
            _stateService.CartUpdated -= OnCartUpdated;
            _cartService.CartUpdated -= OnServiceCartUpdated;
        }

        private class AddonComparer : IEqualityComparer<ServiceAddon>
        {
            public bool Equals(ServiceAddon? x, ServiceAddon? y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;
                return x.Name == y.Name && x.Price == y.Price;
            }

            public int GetHashCode(ServiceAddon obj)
            {
                return HashCode.Combine(obj.Name, obj.Price);
            }
        }

        private Color GetDashboardBackgroundColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#1F2937"),
                Theme.Gray => Color.FromArgb("#F3F4F6"),
                _ => Colors.White
            };
        }
    }
}