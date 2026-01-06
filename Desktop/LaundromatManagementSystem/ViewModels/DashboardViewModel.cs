using System.Windows.Input;
using LaundromatManagementSystem.Models;
using LaundromatManagementSystem.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LaundromatManagementSystem.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly ICartService _cartService;
        private readonly IServiceService _serviceService;
        
        private string _language = "EN";
        private string _theme = "light";
        private string _selectedCategory = "washing";
        private ObservableCollection<CartItem> _cart = new();
        private decimal _subtotal;
        private decimal _tax;
        private decimal _total;
        private bool _showPaymentModal;
        private string _transactionId = string.Empty;
        
        public event PropertyChangedEventHandler? PropertyChanged;
        
        public string Language
        {
            get => _language;
            set
            {
                if (_language != value)
                {
                    _language = value;
                    OnPropertyChanged();
                    
                    // Also update any other components that need language changes
                    UpdateLanguageDependentProperties();
                }
            }
        }
        
        public string Theme
        {
            get => _theme;
            set
            {
                if (_theme != value)
                {
                    _theme = value;
                    OnPropertyChanged();
                    
                    // Update theme resources
                    UpdateThemeResources();
                }
            }
        }
        
        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public ObservableCollection<CartItem> Cart
        {
            get => _cart;
            set
            {
                if (_cart != value)
                {
                    _cart = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public decimal Subtotal
        {
            get => _subtotal;
            set
            {
                if (_subtotal != value)
                {
                    _subtotal = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public decimal Tax
        {
            get => _tax;
            set
            {
                if (_tax != value)
                {
                    _tax = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public decimal Total
        {
            get => _total;
            set
            {
                if (_total != value)
                {
                    _total = value;
                    OnPropertyChanged();
                }
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
        
        public string TransactionId
        {
            get => _transactionId;
            set
            {
                if (_transactionId != value)
                {
                    _transactionId = value;
                    OnPropertyChanged();
                }
            }
        }
        
        // Commands
        public ICommand ChangeLanguageCommand { get; }
        public ICommand ChangeThemeCommand { get; }
        public ICommand ChangeCategoryCommand { get; }
        public ICommand AddToCartCommand { get; }
        public ICommand RemoveFromCartCommand { get; }
        public ICommand UpdateQuantityCommand { get; }
        public ICommand ProcessPaymentCommand { get; }
        public ICommand ClosePaymentModalCommand { get; }
        public ICommand CompletePaymentCommand { get; }
        
        public DashboardViewModel()
        {
        }

        public DashboardViewModel(ICartService cartService, IServiceService serviceService)
        {
            _cartService = cartService;
            _serviceService = serviceService;
            
            // Initialize commands
            ChangeLanguageCommand = new Command<string>(ChangeLanguage);
            ChangeThemeCommand = new Command<string>(ChangeTheme);
            ChangeCategoryCommand = new Command<string>(ChangeCategory);
            AddToCartCommand = new Command<Service>(AddToCart);
            RemoveFromCartCommand = new Command<string>(RemoveFromCart);
            UpdateQuantityCommand = new Command<(string, int)>(UpdateQuantity);
            ProcessPaymentCommand = new Command(ProcessPayment);
            ClosePaymentModalCommand = new Command(ClosePaymentModal);
            CompletePaymentCommand = new Command<(string, string)>(CompletePayment);
            
            // Subscribe to cart updates
            _cartService.CartUpdated += OnCartUpdated;
            
            // Initialize cart from service
            Cart = new ObservableCollection<CartItem>(_cartService.GetCartItems());
            
            CalculateTotals();
        }
        
        private void ChangeLanguage(string language)
        {
            Language = language;
        }
        
        private void ChangeTheme(string theme)
        {
            Theme = theme;
        }
        
        private void ChangeCategory(string category)
        {
            SelectedCategory = category;
        }
        
        private void AddToCart(Service service)
        {
            _cartService.AddToCart(service);
        }
        
        private void RemoveFromCart(string itemId)
        {
            _cartService.RemoveFromCart(itemId);
        }
        
        private void UpdateQuantity((string itemId, int quantity) parameters)
        {
            _cartService.UpdateQuantity(parameters.Item1, parameters.Item2);
        }
        
        private void ProcessPayment()
        {
            if (Cart.Count == 0) return;
            
            ShowPaymentModal = true;
            TransactionId = $"T-{DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()[^6..]}";
        }
        
        private void ClosePaymentModal()
        {
            ShowPaymentModal = false;
        }
        
        private void CompletePayment((string paymentMethod, string customer) parameters)
        {
            // Create transaction
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
            
            // TODO: Add to transaction history
            // _transactionService.AddTransaction(transaction);
            
            // Clear cart
            _cartService.ClearCart();
            
            // Close modal
            ShowPaymentModal = false;
        }
        
        private void OnCartUpdated(object? sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Cart = new ObservableCollection<CartItem>(_cartService.GetCartItems());
                CalculateTotals();
            });
        }
        
        private void CalculateTotals()
        {
            Subtotal = Cart.Sum(item => item.TotalPrice);
            Tax = Math.Round(Subtotal * 0.1m, 2);
            Total = Subtotal + Tax;
        }
        
        private void UpdateLanguageDependentProperties()
        {
            // Update any properties that depend on language
            // This will trigger UI updates through bindings
        }
        
        private void UpdateThemeResources()
        {
            // Update application resources based on theme
            // Implementation depends on how you handle themes
            
            // For now, we'll just trigger property change
            OnPropertyChanged(nameof(Theme));
        }
        
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}