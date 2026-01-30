using LaundromatManagementSystem.Services;
using LaundromatManagementSystem.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace LaundromatManagementSystem
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

        public App()
        {
            InitializeComponent();
            
            // Initialize database
            Task.Run(async () => await InitializeDatabase());

            MainPage = new AppShell();
        }

        private async Task InitializeDatabase()
        {
            try
            {
                using var scope = Services.CreateScope();
                var serviceService = scope.ServiceProvider.GetRequiredService<IServiceService>();
                await serviceService.InitializeDatabaseAsync();
            }
            catch (Exception ex)
            {
                // Since MainPage might not be ready yet, we'll log this error
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
            }
        }

        public static void InitializeServiceProvider(IServiceProvider serviceProvider)
        {
            Services = serviceProvider;
        }
    }

    public static class ServiceLocator
    {
        public static T GetService<T>() where T : class => App.Services.GetService<T>();
    }

    // Enums matching appearance and usage in the application
    public enum Theme { Light, Gray, Dark }
    public enum Language { EN, RW, FR }
}