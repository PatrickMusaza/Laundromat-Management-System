using LaundromatManagementSystem.ViewModels;
using LaundromatManagementSystem.Services;

namespace LaundromatManagementSystem.Views
{
    public partial class Dashboard : ContentPage
    {
        public Dashboard()
        {
            InitializeComponent();

            // Create ViewModel with dependencies
            var cartService = new CartService();
            var serviceService = new ServiceService();
            var viewModel = new DashboardViewModel(serviceService);

            BindingContext = viewModel;

            // Subscribe to cart updates to refresh the shopping cart
            cartService.CartUpdated += (sender, e) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    viewModel.Cart = new System.Collections.ObjectModel.ObservableCollection<Models.CartItem>(cartService.GetCartItems());
                });
            };
        }
    }
}