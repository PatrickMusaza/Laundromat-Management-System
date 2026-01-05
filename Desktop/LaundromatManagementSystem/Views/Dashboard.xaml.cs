using CommunityToolkit.Mvvm.Input;
using LaundromatManagementSystem.Models;
using LaundromatManagementSystem.ViewModels;

namespace LaundromatManagementSystem.Views
{
    public partial class Dashboard : ContentPage
    {
        private DashboardViewModel _viewModel;
        
        public Dashboard()
        {
            InitializeComponent();
            _viewModel = BindingContext as DashboardViewModel;
        }
        
        private void OnLanguageChanged(string language)
        {
            _viewModel?.ChangeLanguage(language);
        }
        
        private void OnThemeChanged(string theme)
        {
            _viewModel?.ChangeTheme(theme);
        }
        
        private void OnCategoryChanged(string category)
        {
            _viewModel?.ChangeCategory(category);
        }
    }
}