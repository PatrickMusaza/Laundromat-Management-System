using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace LaundromatManagementSystem.WinUI
{
    public partial class App : MauiWinUIApplication
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
        
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);
            
            // Set window size for desktop
            var currentWindow = Application.Windows[0].Handler.PlatformView;
            var windowHandle = WindowNative.GetWindowHandle(currentWindow);
            var windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            
            appWindow.Resize(new Windows.Graphics.SizeInt32(1200, 800));
            appWindow.Title = "Laundromat Management System";
        }
    }
}