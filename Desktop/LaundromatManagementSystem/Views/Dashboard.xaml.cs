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
            var viewModel = new DashboardViewModel(cartService, serviceService);
            
            // Add debug output to see if commands work
            viewModel.ChangeLanguageCommand.CanExecuteChanged += (s, e) => 
                Console.WriteLine($"ChangeLanguageCommand CanExecute changed");
            
            viewModel.ChangeThemeCommand.CanExecuteChanged += (s, e) => 
                Console.WriteLine($"ChangeThemeCommand CanExecute changed");
            
            BindingContext = viewModel;
        }
        
        // Optional: Override OnAppearing to debug
        protected override void OnAppearing()
        {
            base.OnAppearing();
            Console.WriteLine("Dashboard appeared");
            
            if (BindingContext is DashboardViewModel vm)
            {
                Console.WriteLine($"Current Language: {vm.Language}, Theme: {vm.Theme}");
            }
        }
    }
}