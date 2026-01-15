using LaundromatManagementSystem.ViewModels;

namespace LaundromatManagementSystem.Views;

public partial class Footer : ContentView
{
    public Footer()
    {
        InitializeComponent();
        BindingContext = new FooterViewModel();
    }
}
