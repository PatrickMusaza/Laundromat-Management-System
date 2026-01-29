using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LaundromatManagementSystem.Models;
using LaundromatManagementSystem.Services;

namespace LaundromatManagementSystem.Services
{
    public class ApplicationStateService : INotifyPropertyChanged
    {
        private static ApplicationStateService _instance;
        public static ApplicationStateService Instance => _instance ??= new ApplicationStateService();
        
        private Theme _currentTheme = Theme.Light;
        private Language _currentLanguage = Language.EN;
        private ObservableCollection<CartItem> _cartItems = new();
        private bool _showPaymentModal;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public double CartTotal =>(double) CartItems.Sum(item => item.TotalPrice);
        public int ItemCount => CartItems.Sum(item => item.Quantity);

        public event EventHandler<Theme> ThemeChanged;
        public event EventHandler<Language> LanguageChanged;
        public event EventHandler CartUpdated;

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
                    ServiceType = item.ServiceType
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
    }
}