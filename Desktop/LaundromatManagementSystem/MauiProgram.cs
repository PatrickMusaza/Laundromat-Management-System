using Microsoft.Extensions.Logging;
using LaundromatManagementSystem.Data;
using LaundromatManagementSystem.Repositories;
using LaundromatManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using LaundromatManagementSystem.ViewModels;

namespace LaundromatManagementSystem;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("segoeuithis.ttf", "SegoeUIRegular");
				fonts.AddFont("segoeuithibd.ttf", "SegoeUISemibold");
				fonts.AddFont("segoeuithisi.ttf", "SegoeUIItalic");
				fonts.AddFont("segoeuithisz.ttf", "SegoeUIBoldItalic");
			});

		// Configure Dependency Injection
		ConfigureServices(builder.Services);

#if DEBUG
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();

		// Initialize the static service provider in App class
		App.InitializeServiceProvider(app.Services);

		return app;
	}
	
	private static void ConfigureServices(IServiceCollection services)
	{
		// Register DbContext
		services.AddDbContext<AppDbContext>(options =>
		{
			string databasePath = GetDatabasePath();
			var connectionString = $"Data Source={databasePath};";
			options.UseSqlite(connectionString);
		});

		// Register repositories
		services.AddScoped<IServiceRepository, ServiceRepository>();
		services.AddScoped<ITransactionRepository, TransactionRepository>();

		// Register services
		services.AddTransient<IServiceService, ServiceService>();
		services.AddTransient<ITransactionService, TransactionService>();
		services.AddTransient<ICartService, CartService>();
		services.AddTransient<IPrinterService, PrinterService>();

		// Register ViewModels - PaymentModalViewModel without commands in DI
		services.AddTransient<ServiceGridViewModel>();
		services.AddTransient<ServiceViewModel>();
		services.AddTransient<CategoryViewModel>();
		services.AddTransient<DashboardViewModel>();

		// Register state services
		services.AddSingleton<ApplicationStateService>();
	}

	private static string GetDatabasePath()
	{
		return Path.Combine(FileSystem.AppDataDirectory, "laundromat.db3");
	}
}