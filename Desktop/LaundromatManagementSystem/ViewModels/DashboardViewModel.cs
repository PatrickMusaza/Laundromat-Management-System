using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaundromatManagementSystem.Models;
using LaundromatManagementSystem.Services;

namespace LaundromatManagementSystem.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly ICartService _cartService;
        private readonly IServiceService _serviceService;

        [ObservableProperty]
        private string _language = "EN";

        [ObservableProperty]
        private string _theme = "light";

        [ObservableProperty]
        private string _selectedCategory = "washing";

        public List<CartItem> Cart { get; set; } = new();

        [ObservableProperty]
        private decimal _subtotal;

        [ObservableProperty]
        private decimal _tax;

        [ObservableProperty]
        private decimal _total;

        [ObservableProperty]
        private bool _showPaymentModal;

        [ObservableProperty]
        private string _transactionId;

        public DashboardViewModel()
        {
            // default implementations
            _cartService = new CartService();
            _serviceService = new ServiceService();

            _transactionId = string.Empty;

            _cartService.CartUpdated += OnCartUpdated;

            LoadInitialData();
            CalculateTotals();
        }

        public DashboardViewModel(ICartService cartService, IServiceService serviceService)
        {
            _cartService = cartService;
            _serviceService = serviceService;

            _cartService.CartUpdated += OnCartUpdated;

            LoadInitialData();
            CalculateTotals();
        }

        #region Commands

        [RelayCommand]
        public void ChangeLanguage(string language)
        {
            Language = language;
        }

        [RelayCommand]
        public void ChangeTheme(string theme)
        {
            Theme = theme;
            UpdateThemeResources();
        }

        [RelayCommand]
        public void ChangeCategory(string category)
        {
            SelectedCategory = category;
            _ = LoadServicesForCategory(category);
        }

        [RelayCommand]
        private void AddToCart(Service service)
        {
            _cartService.AddToCart(service);
        }

        [RelayCommand]
        private void RemoveFromCart(int itemId)
        {
            _cartService.RemoveFromCart(itemId);
        }

        [RelayCommand]
        private void UpdateQuantity((string itemId, int quantity) parameters)
        {
            _cartService.UpdateQuantity(parameters.itemId, parameters.quantity);
        }

        [RelayCommand]
        private void ProcessPayment()
        {
            if (Cart.Count == 0) return;

            ShowPaymentModal = true;
            TransactionId = $"T-{DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString().Substring(6)}";
        }

        [RelayCommand]
        private void ClosePaymentModal()
        {
            ShowPaymentModal = false;
        }

        [RelayCommand]
        private void CompletePayment((string paymentMethod, string customer) parameters)
        {
            var transaction = new Transaction
            {
                Id = TransactionId,
                Timestamp = DateTime.UtcNow,
                Amount = Total,
                PaymentMethod = parameters.paymentMethod,
                Status = "completed",
                Items = new List<CartItem>(Cart),
                Customer = parameters.customer
            };

            // Save transaction (implement your service)
            // _transactionService.AddTransaction(transaction);

            _cartService.ClearCart();
            ShowPaymentModal = false;
        }

        #endregion

        #region Private Methods

        private void OnCartUpdated(object sender, EventArgs e)
        {
            Cart = _cartService.GetCartItems();
            CalculateTotals();
        }

        private void CalculateTotals()
        {
            Subtotal = Cart.Sum(item => item.TotalPrice);
            Tax = Math.Round(Subtotal * 0.1m, 2);
            Total = Subtotal + Tax;
        }

        private void LoadInitialData()
        {
            _ = LoadServicesForCategory(SelectedCategory);
        }

        private async Task LoadServicesForCategory(string category)
        {
            var services = await _serviceService.GetServicesByCategoryAsync(category);
            // TODO: bind services to UI (if needed)
        }

        private void UpdateThemeResources()
        {
            var app = Application.Current;

            if (Theme == "dark")
            {
                // Set dark theme resources
            }
            else if (Theme == "gray")
            {
                // Set gray theme resources
            }
            else
            {
                // Set light theme resources
            }
        }

        #endregion
    }
}
