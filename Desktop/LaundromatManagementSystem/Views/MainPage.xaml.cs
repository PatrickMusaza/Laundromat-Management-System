using LaundromatManagementSystem.ViewModels;

namespace LaundromatManagementSystem.Views;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        BindingContext = ServiceLocator.GetService<MainViewModel>();
    }
}
