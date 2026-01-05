using Microsoft.Maui.Controls;

namespace LaundromatManagementSystem.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnWashTapped(object sender, System.EventArgs e)
        {
            // Add ripple effect
            var border = (Border)sender;
            await border.ScaleTo(0.95, 50, Easing.Linear);
            await border.ScaleTo(1, 50, Easing.Linear);
            
            // Navigate to Wash services page
            await DisplayAlert("WASH", "Wash services selected", "OK");
        }

        private async void OnHotWaterTapped(object sender, System.EventArgs e)
        {
            // Add ripple effect
            var border = (Border)sender;
            await border.ScaleTo(0.95, 50, Easing.Linear);
            await border.ScaleTo(1, 50, Easing.Linear);
            
            // Add Hot Water service to cart
            await DisplayAlert("Added", "Hot Water service added to cart", "OK");
        }

        private async void OnLanguageTapped(object sender, System.EventArgs e)
        {
            var border = (Border)sender;
            var label = (Label)border.Content;
            
            // Reset all language buttons
            ResetLanguageButtons();
            
            // Set selected language
            border.BackgroundColor = Color.FromArgb("#4A90E2");
            label.TextColor = Colors.White;
            
            // Apply language change
            string language = label.Text;
            await DisplayAlert("Language", $"Language changed to {language}", "OK");
        }

        private void ResetLanguageButtons()
        {
            // This would need to be implemented to reset all language buttons to default state
            // For now, we'll just show an alert
        }
    }
}