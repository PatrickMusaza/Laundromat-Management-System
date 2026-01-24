using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaundromatManagementSystem.Services;

namespace LaundromatManagementSystem.ViewModels
{
    public partial class CustomerHeaderViewModel : ObservableObject
    {
        private readonly ApplicationStateService _stateService = ApplicationStateService.Instance;

        // Keep settable properties for XAML binding
        [ObservableProperty]
        private Language _language;

        [ObservableProperty]
        private Theme _theme;

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
                ["theme"] = "Appearance",
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

        public CustomerHeaderViewModel()
        {
            // Initialize from state service
            _language = _stateService.CurrentLanguage;
            _theme = _stateService.CurrentTheme;

            // Subscribe to state changes
            _stateService.PropertyChanged += OnStateChanged;
            UpdateTranslations();
        }

        // Override setters to update state service
        partial void OnLanguageChanged(Language value)
        {
            if (_stateService.CurrentLanguage != value)
            {
                _stateService.CurrentLanguage = value;
            }
            UpdateTranslations();
        }

        partial void OnThemeChanged(Theme value)
        {
            if (_stateService.CurrentTheme != value)
            {
                _stateService.CurrentTheme = value;
            }
            UpdateTranslations();
        }

        private void OnStateChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                switch (e.PropertyName)
                {
                    case nameof(_stateService.CurrentLanguage):
                        if (Language != _stateService.CurrentLanguage)
                        {
                            Language = _stateService.CurrentLanguage;
                        }
                        break;

                    case nameof(_stateService.CurrentTheme):
                        if (Theme != _stateService.CurrentTheme)
                        {
                            Theme = _stateService.CurrentTheme;
                        }
                        break;
                }
            });
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

        // Clean up
        public void Cleanup()
        {
            _stateService.PropertyChanged -= OnStateChanged;
        }
    }
}