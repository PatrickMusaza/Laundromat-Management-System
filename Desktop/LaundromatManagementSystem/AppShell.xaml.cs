#if WINDOWS
using Microsoft.UI.Windowing;
#endif

namespace LaundromatManagementSystem;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Subscribe to Loaded event correctly
        Loaded += OnLoaded;
    }

#if WINDOWS
    private void OnLoaded(object sender, EventArgs e)
    {
        var window = this.Window;
        if (window?.Handler?.PlatformView is Microsoft.UI.Xaml.Window nativeWindow)
        {
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            // Fullscreen, no title bar, no buttons
            appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
        }
    }
#endif
}
