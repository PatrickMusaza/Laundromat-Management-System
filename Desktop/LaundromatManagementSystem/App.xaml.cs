using LaundromatManagementSystem.Services;
using LaundromatManagementSystem.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LaundromatManagementSystem
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        public static IServiceProvider Services { get; private set; }

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;

            // Initialize database
            Task.Run(async () => await InitializeDatabase());

            //Configure services
            ConfigureServices();

            MainPage = new AppShell();
        }

        private void ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register services
            services.AddSingleton<IServiceService, ServiceService>();
            services.AddSingleton<ICartService, CartService>();
            services.AddSingleton<IPrinterService, PrinterService>();

            // Register ViewModels
            services.AddSingleton<DashboardViewModel>();

            Services = services.BuildServiceProvider();
        }

        private async Task InitializeDatabase()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var serviceService = scope.ServiceProvider.GetRequiredService<IServiceService>();
                await serviceService.InitializeDatabaseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization error: {ex.Message}");
            }
        }
    }

    public static class ServiceLocator
    {
        public static T GetService<T>() where T : class => App.Services.GetService<T>();
    }

    // Enums matching appearance and usage in the application
    public enum Theme { Light, Gray, Dark }
    public enum Language { EN, RW, FR }
    public enum PaymentMethod { Cash, MoMo, Card }
}