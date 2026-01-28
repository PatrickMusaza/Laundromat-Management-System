using LaundromatManagementSystem.ViewModels;
using LaundromatManagementSystem.Services;
using LaundromatManagementSystem.Repositories;
using LaundromatManagementSystem.Data;

namespace LaundromatManagementSystem.Views
{
    public partial class Dashboard : ContentPage
    {
        private readonly AppDbContext _context;

        public Dashboard()
        {
            InitializeComponent();

            // Create ViewModel with dependencies
            var cartService = new CartService();
            var repository = new ServiceRepository(_context);
            var serviceService = new ServiceService(repository);
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