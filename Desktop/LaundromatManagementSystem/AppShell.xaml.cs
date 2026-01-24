#if WINDOWS
using Microsoft.UI.Windowing;
#endif

namespace LaundromatManagementSystem;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

#if WINDOWS
        Loaded += OnLoaded;
#endif
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

            appWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
        }
    }
#endif
}
