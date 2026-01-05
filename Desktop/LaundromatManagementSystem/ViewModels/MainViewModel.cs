using CommunityToolkit.Mvvm.ComponentModel;
using LaundromatManagementSystem.Services;

namespace LaundromatManagementSystem.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ICartService _cartService;
        
        [ObservableProperty]
        private string _welcomeMessage = "Welcome to Laundromat";
        
        [ObservableProperty]
        private string _cartSummary;
        
        [ObservableProperty]
        private decimal _cartTotal;
        
        public MainViewModel(ICartService cartService)
        {
            _cartService = cartService;
            UpdateCartSummary();
            
            _cartService.CartUpdated += (s, e) => UpdateCartSummary();
        }
        
        private void UpdateCartSummary()
        {
            CartTotal = _cartService.GetTotalAmount();
            var itemCount = _cartService.GetItemCount();
            
            if (itemCount == 0)
            {
                CartSummary = "Your cart is empty";
            }
            else
            {
                CartSummary = $"{itemCount} item(s) - RWF {CartTotal:N0}";
            }
            
            OnPropertyChanged(nameof(CartSummary));
            OnPropertyChanged(nameof(CartTotal));
        }
    }
}