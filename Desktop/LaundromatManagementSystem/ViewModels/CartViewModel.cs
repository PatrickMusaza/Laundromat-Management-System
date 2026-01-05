using CommunityToolkit.Mvvm.ComponentModel;
using LaundromatManagementSystem.Services;

namespace LaundromatManagementSystem.ViewModels
{
    public partial class CartViewModel : ObservableObject
    {
        private readonly ICartService _cartService;

        public CartViewModel(ICartService cartService)
        {
            _cartService = cartService;
        }
    }
}
