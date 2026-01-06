using Microsoft.Extensions.DependencyInjection;
using LaundromatManagementSystem.Services;
using LaundromatManagementSystem.ViewModels;
using LaundromatManagementSystem.Views;

namespace LaundromatManagementSystem
{
    public partial class App : Application
    {
        public static IServiceProvider? Services { get; private set; }
        
        public App()
        {
            InitializeComponent();
            
            // Set up dependency injection
            ConfigureServices();
            
            // Create main window
            MainPage = new AppShell();
        }
        
        private void ConfigureServices()
        {
            var services = new ServiceCollection();
            
            // Register services
            services.AddSingleton<IServiceService, ServiceService>();
            services.AddSingleton<ICartService, CartService>();
            
            // Register ViewModels
            services.AddSingleton<DashboardViewModel>();
            
            Services = services.BuildServiceProvider();
        }
    }
}