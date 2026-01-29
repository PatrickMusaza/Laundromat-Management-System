using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaundromatManagementSystem.Models;
using LaundromatManagementSystem.Services;
using System.Collections.ObjectModel;

namespace LaundromatManagementSystem.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        private readonly ApplicationStateService _stateService = ApplicationStateService.Instance;
        private readonly IServiceService _serviceService;

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
        private string _transactionId = string.Empty;

        [ObservableProperty]
        private Color _dashboardBackgroundColor;

        public bool ShowPaymentModal => _stateService.ShowPaymentModal;

        public DashboardViewModel(IServiceService serviceService)
        {
            _serviceService = serviceService;

            // Initialize from state service
            _language = _stateService.CurrentLanguage;
            _theme = _stateService.CurrentTheme;
            Cart = new ObservableCollection<CartItem>(_stateService.CartItems);
            CalculateTotals();
            DashboardBackgroundColor = GetDashboardBackgroundColor();

            // Subscribe to state changes
            _stateService.PropertyChanged += OnStateChanged;
            _stateService.CartUpdated += OnCartUpdated;
        }

        partial void OnLanguageChanged(Language value) => _stateService.CurrentLanguage = value;
        partial void OnThemeChanged(Theme value)
        {
            _stateService.CurrentTheme = value;
            DashboardBackgroundColor = GetDashboardBackgroundColor();
        }

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
                        break;

                    case nameof(_stateService.CartItems):
                        RefreshCart();
                        break;

                    case nameof(_stateService.ShowPaymentModal):
                        OnPropertyChanged(nameof(ShowPaymentModal));
                        break;
                }
            });
        }

        [RelayCommand]
        private void ChangeCategory(string category) => SelectedCategory = category;

        private void RefreshCart()
        {
            // Update the Cart collection
            Cart = new ObservableCollection<CartItem>(_stateService.CartItems);
            CalculateTotals();
        }

        [RelayCommand]
        private async Task ProcessPayment()
        {
            if (Cart.Count == 0) return;

            // Show modal via state service
            _stateService.ShowPaymentModal = true;
            TransactionId = $"T-{DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString()[^6..]}";

            await Application.Current.MainPage.DisplayAlert("Payment", "Proceed to payment modal.", "OK");
        }

        [RelayCommand]
        private async Task ClosePaymentModal()
        {
            // Close modal via state service
            _stateService.ShowPaymentModal = false;
            await Application.Current.MainPage.DisplayAlert("Payment", "Payment modal closed.", "OK");
        }

        private void OnCartUpdated(object? sender, EventArgs e) => RefreshCart();

        private void CalculateTotals()
        {
            Subtotal = Cart.Sum(item => item.TotalPrice);
            Tax = Math.Round(Subtotal * 0.1m, 2);
            Total = Subtotal + Tax;
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

        public void Cleanup()
        {
            _stateService.PropertyChanged -= OnStateChanged;
            _stateService.CartUpdated -= OnCartUpdated;
        }
    }
}