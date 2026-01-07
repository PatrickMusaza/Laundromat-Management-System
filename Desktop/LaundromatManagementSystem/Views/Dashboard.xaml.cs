using LaundromatManagementSystem.ViewModels;
using LaundromatManagementSystem.Services;

namespace LaundromatManagementSystem.Views
{
    public partial class Dashboard : ContentPage
    {
        private ShoppingCart _shoppingCart;
        
        public Dashboard()
        {
            InitializeComponent();
            
            // Create ViewModel
            var cartService = new CartService();
            var serviceService = new ServiceService();
            var viewModel = new DashboardViewModel(cartService, serviceService);
            
            BindingContext = viewModel;
            
            // Find ShoppingCart view
            Loaded += OnDashboardLoaded;
        }
        
        private void OnDashboardLoaded(object sender, EventArgs e)
        {
            // Find the ShoppingCart view
            _shoppingCart = this.FindByName<ShoppingCart>("ShoppingCart");
        }
        
        // This method is called from ServiceGrid when a service is tapped
        public void OnServiceTapped(Models.CartItem cartItem)
        {
            _shoppingCart?.AddItem(cartItem);
            
            // Also update the ViewModel
            if (BindingContext is DashboardViewModel vm)
            {
                vm.AddToCartCommand.Execute(cartItem);
            }
        }
    }
}