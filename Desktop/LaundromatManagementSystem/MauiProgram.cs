using Microsoft.Extensions.Logging;
using LaundromatManagementSystem.Data;
using LaundromatManagementSystem.Repositories;
using LaundromatManagementSystem.Services;
using Microsoft.EntityFrameworkCore;

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

		//builder.UseMauiCommunityToolkit();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}


	private static void ConfigureServices(IServiceCollection services)
	{
		// Register DbContext
		services.AddDbContext<AppDbContext>(options =>
		{
			string databasePath = GetDatabasePath();
			var connectionString = $"Data Source={databasePath};Password=SecurePassword123!";
			options.UseSqlite(connectionString);
		});

		// Register repositories
		services.AddScoped<IServiceRepository, ServiceRepository>();

		// Register services
		services.AddScoped<IServiceService, ServiceService>();

		// Register ViewModels
		services.AddTransient<ViewModels.ServiceGridViewModel>();
		services.AddTransient<ViewModels.ServiceViewModel>();

		// Other services...
		services.AddSingleton<ApplicationStateService>();
	}

	private static string GetDatabasePath()
	{
		return Path.Combine(FileSystem.AppDataDirectory, "laundromat.db3");
	}
}
