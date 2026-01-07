using Microsoft.Extensions.DependencyInjection;
using LaundromatManagementSystem.Services;
using LaundromatManagementSystem.ViewModels;

namespace LaundromatManagementSystem
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }
        
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
            services.AddTransient<IServiceService, ServiceService>();
            services.AddTransient<ICartService, CartService>();
            
            // Register ViewModels
            services.AddTransient<DashboardViewModel>();
            
            Services = services.BuildServiceProvider();
        }
    }
    
    public static class ServiceLocator
    {
        public static T GetService<T>() where T : class => App.Services.GetService<T>();
    }
    
    // Enums matching TypeScript
    public enum Theme { Light, Gray, Dark }
    public enum Language { EN, RW, FR }
    public enum PaymentMethod { Cash, MoMo, Card }
}