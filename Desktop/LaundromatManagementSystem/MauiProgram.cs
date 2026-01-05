using Microsoft.Extensions.Logging;

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

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
