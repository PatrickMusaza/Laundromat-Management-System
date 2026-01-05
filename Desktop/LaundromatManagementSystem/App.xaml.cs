using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LaundromatManagementSystem.Services;
using LaundromatManagementSystem.ViewModels;

namespace LaundromatManagementSystem
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            
            // Set up dependency injection
            ConfigureServices();
            
            MainPage = new AppShell();
        }
        
        private void ConfigureServices()
        {
            var services = new ServiceCollection();
            
            // Register services
            services.AddSingleton<IServiceService, ServiceService>();
            services.AddSingleton<ICartService, CartService>();
            
            // Register ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<ServicesViewModel>();
            services.AddTransient<CartViewModel>();
            
            ServiceLocator.ServiceProvider = services.BuildServiceProvider();
        }
    }
}
