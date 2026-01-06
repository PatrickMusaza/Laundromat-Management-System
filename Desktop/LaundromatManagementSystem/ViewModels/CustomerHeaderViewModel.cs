using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LaundromatManagementSystem.ViewModels
{
    public partial class CustomerHeaderViewModel : ObservableObject
    {
        [ObservableProperty]
        private Language _language = Language.EN;
        
        [ObservableProperty]
        private Theme _theme = Theme.Light;
        
        [ObservableProperty]
        private string _welcomeText = "Welcome to Laundromat";
        
        [ObservableProperty]
        private string _selectServiceText = "Select Your Services";
        
        [ObservableProperty]
        private string _languageLabelText = "Language";
        
        [ObservableProperty]
        private string _themeLabelText = "Appearance";
        
        [ObservableProperty]
        private string _currentLanguage = "EN";
        
        [ObservableProperty]
        private string _currentThemeText = "Light";
        
        [ObservableProperty]
        private string _themeIcon = "☀️";
        
        public ICommand LanguageChangedCommand { get; }
        public ICommand ThemeChangedCommand { get; }
        
        private readonly Dictionary<Language, Dictionary<string, string>> _translations = new()
        {
            [Language.EN] = new()
            {
                ["welcome"] = "Welcome to Laundromat",
                ["selectService"] = "Select Your Services",
                ["language"] = "Language",
                ["theme"] = "Appearance",
                ["light"] = "Light",
                ["gray"] = "Gray",
                ["dark"] = "Dark"
            },
            [Language.RW] = new()
            {
                ["welcome"] = "Murakaza Neza",
                ["selectService"] = "Hitamo Serivisi Zawe",
                ["language"] = "Ururimi",
                ["theme"] = "Isura",
                ["light"] = "Yera",
                ["gray"] = "Icyatsi",
                ["dark"] = "Umwijima"
            },
            [Language.FR] = new()
            {
                ["welcome"] = "Bienvenue à la Laverie",
                ["selectService"] = "Sélectionnez Vos Services",
                ["language"] = "Langue",
                ["theme"] = "Apparence",
                ["light"] = "Clair",
                ["gray"] = "Gris",
                ["dark"] = "Sombre"
            }
        };
        
        private readonly Dictionary<Theme, string> _themeIcons = new()
        {
            [Theme.Light] = "☀️",
            [Theme.Gray] = "☁️",
            [Theme.Dark] = "🌙"
        };
        
        public CustomerHeaderViewModel(ICommand languageChangedCommand, ICommand themeChangedCommand)
        {
            LanguageChangedCommand = languageChangedCommand;
            ThemeChangedCommand = themeChangedCommand;
            UpdateTranslations();
        }
        
        partial void OnLanguageChanged(Language value)
        {
            UpdateTranslations();
            CurrentLanguage = value.ToString();
            LanguageChangedCommand?.Execute(value);
        }
        
        partial void OnThemeChanged(Theme value)
        {
            UpdateTranslations();
            CurrentThemeText = _translations[Language][value.ToString().ToLower()];
            ThemeIcon = _themeIcons[value];
            ThemeChangedCommand?.Execute(value);
        }
        
        private void UpdateTranslations()
        {
            var t = _translations[Language];
            WelcomeText = t["welcome"];
            SelectServiceText = t["selectService"];
            LanguageLabelText = t["language"];
            ThemeLabelText = t["theme"];
            CurrentThemeText = t[Theme.ToString().ToLower()];
            ThemeIcon = _themeIcons[Theme];
        }
        
        [RelayCommand]
        private async Task ShowLanguagePopup()
        {
            var languages = new[] { Language.EN, Language.RW, Language.FR };
            var languageNames = new[] { "English", "Kinyarwanda", "Français" };
            
            var action = await Shell.Current.DisplayActionSheet(
                "Select Language",
                "Cancel",
                null,
                languageNames
            );
            
            if (action != null && action != "Cancel")
            {
                var selectedIndex = Array.IndexOf(languageNames, action);
                if (selectedIndex >= 0)
                {
                    Language = languages[selectedIndex];
                }
            }
        }
        
        [RelayCommand]
        private async Task ShowThemePopup()
        {
            var themes = new[] { Theme.Light, Theme.Gray, Theme.Dark };
            var themeNames = new[] 
            { 
                _translations[Language]["light"],
                _translations[Language]["gray"],
                _translations[Language]["dark"]
            };
            
            var action = await Shell.Current.DisplayActionSheet(
                "Select Theme",
                "Cancel",
                null,
                themeNames
            );
            
            if (action != null && action != "Cancel")
            {
                var selectedIndex = Array.IndexOf(themeNames, action);
                if (selectedIndex >= 0)
                {
                    Theme = themes[selectedIndex];
                }
            }
        }
    }
}