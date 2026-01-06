using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaundromatManagementSystem.Models;
using LaundromatManagementSystem.Services;
using System.Collections.ObjectModel;

namespace LaundromatManagementSystem.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private Language _language = Language.EN;

        [ObservableProperty]
        private Theme _theme = Theme.Light;

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

        public DashboardViewModel(ICartService cartService, IServiceService serviceService)
        {
            _cartService = cartService;
            _serviceService = serviceService;

            _cartService.CartUpdated += OnCartUpdated;
            Cart = new ObservableCollection<CartItem>(_cartService.GetCartItems());
            CalculateTotals();
        }

        [RelayCommand]
        private void ChangeLanguage(Language language) => Language = language;

        [RelayCommand]
        private void ChangeTheme(Theme theme) => Theme = theme;

        [RelayCommand]
        private void ChangeCategory(string category) => SelectedCategory = category;

        [RelayCommand]
        private void AddToCart(CartItem item)
        {
            var existing = Cart.FirstOrDefault(i =>
                i.Name == item.Name &&
                i.Addons.SequenceEqual(item.Addons, new AddonComparer()));

            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                Cart.Add(new CartItem
                {
                    Id = $"{item.Name}-{Guid.NewGuid():N}",
                    Name = item.Name,
                    Price = item.Price,
                    Addons = new ObservableCollection<ServiceAddon>(item.Addons),
                    Quantity = 1
                });
            }

            CalculateTotals();
            _cartService.CartUpdated += OnCartUpdated;
        }

        [RelayCommand]
        private void RemoveFromCart(string itemId)
        {
            var item = Cart.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                Cart.Remove(item);
                CalculateTotals();
                _cartService.CartUpdated += OnCartUpdated;
            }
        }

        [RelayCommand]
        private void UpdateQuantity((string itemId, int quantity) parameters)
        {
            if (parameters.quantity <= 0)
            {
                RemoveFromCart(parameters.itemId);
                return;
            }

            var item = Cart.FirstOrDefault(i => i.Id == parameters.itemId);
            if (item != null)
            {
                item.Quantity = parameters.quantity;
                CalculateTotals();
                _cartService.CartUpdated += OnCartUpdated;
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

            Cart.Clear();
            CalculateTotals();
            ShowPaymentModal = false;
            _cartService.CartUpdated += OnCartUpdated;
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
    }
}