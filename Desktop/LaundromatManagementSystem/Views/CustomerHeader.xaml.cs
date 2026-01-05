using System.Windows.Input;

namespace LaundromatManagementSystem.Views
{
    public partial class CustomerHeader : ContentView
    {
        public static readonly BindableProperty LanguageProperty =
            BindableProperty.Create(nameof(Language), typeof(string), typeof(CustomerHeader), "EN");
        
        public static readonly BindableProperty ThemeProperty =
            BindableProperty.Create(nameof(Theme), typeof(string), typeof(CustomerHeader), "dark");
        
        public static readonly BindableProperty LanguageChangedProperty =
            BindableProperty.Create(nameof(LanguageChanged), typeof(ICommand), typeof(CustomerHeader));
        
        public static readonly BindableProperty ThemeChangedProperty =
            BindableProperty.Create(nameof(ThemeChanged), typeof(ICommand), typeof(CustomerHeader));
        
        public string Language
        {
            get => (string)GetValue(LanguageProperty);
            set => SetValue(LanguageProperty, value);
        }
        
        public string Theme
        {
            get => (string)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }
        
        public ICommand LanguageChanged
        {
            get => (ICommand)GetValue(LanguageChangedProperty);
            set => SetValue(LanguageChangedProperty, value);
        }
        
        public ICommand ThemeChanged
        {
            get => (ICommand)GetValue(ThemeChangedProperty);
            set => SetValue(ThemeChangedProperty, value);
        }
        
        private Dictionary<string, Dictionary<string, string>> _translations = new()
        {
            ["EN"] = new()
            {
                ["welcome"] = "Welcome to Laundromat",
                ["selectService"] = "Select Your Services",
                ["language"] = "Language",
                ["theme"] = "Appearance",
                ["light"] = "Light",
                ["gray"] = "Gray",
                ["dark"] = "Dark"
            },
            ["RW"] = new()
            {
                ["welcome"] = "Murakaza Neza",
                ["selectService"] = "Hitamo Serivisi Zawe",
                ["language"] = "Ururimi",
                ["theme"] = "Isura",
                ["light"] = "Yera",
                ["gray"] = "Icyatsi",
                ["dark"] = "Umwijima"
            },
            ["FR"] = new()
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
        
        private Dictionary<string, string> _themeIcons = new()
        {
            ["light"] = "☀️",
            ["gray"] = "☁️",
            ["dark"] = "🌙"
        };
        
        public CustomerHeader()
        {
            InitializeComponent();
            UpdateLanguage();
            UpdateTheme();
        }
        
        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            
            if (propertyName == nameof(Language))
            {
                UpdateLanguage();
            }
            else if (propertyName == nameof(Theme))
            {
                UpdateTheme();
            }
        }
        
        private void UpdateLanguage()
        {
            var t = _translations[Language];
            
            WelcomeLabel.Text = t["welcome"];
            SelectServiceLabel.Text = t["selectService"];
            LanguageLabel.Text = t["language"];
            ThemeLabel.Text = t["theme"];
            CurrentLanguageLabel.Text = Language;
            CurrentThemeLabel.Text = t[Theme];
        }
        
        private void UpdateTheme()
        {
            var t = _translations[Language];
            
            // Update theme icon
            ThemeIconLabel.Text = _themeIcons[Theme];
            CurrentThemeLabel.Text = t[Theme];
            
            // Apply theme colors through DynamicResource
            UpdateThemeColors();
        }
        
        private void UpdateThemeColors()
        {
            // These colors will be set in App.xaml resources
            // based on the selected theme
        }
        
        public async void OnLanguageTapped(object sender, System.EventArgs e)
        {
            var languages = new[] { "EN", "RW", "FR" };
            var languageNames = new[] { "English", "Kinyarwanda", "Français" };
            
            var result = await Application.Current.MainPage.DisplayActionSheet(
                "Select Language",
                "Cancel",
                null,
                languageNames
            );
            
            if (result != null && result != "Cancel")
            {
                var selectedIndex = Array.IndexOf(languageNames, result);
                if (selectedIndex >= 0)
                {
                    var newLanguage = languages[selectedIndex];
                    LanguageChanged?.Execute(newLanguage);
                }
            }
        }
        
        public async void OnThemeTapped(object sender, System.EventArgs e)
        {
            var t = _translations[Language];
            var themes = new[] { "light", "gray", "dark" };
            var themeNames = new[] { t["light"], t["gray"], t["dark"] };
            
            var result = await Application.Current.MainPage.DisplayActionSheet(
                "Select Theme",
                "Cancel",
                null,
                themeNames
            );
            
            if (result != null && result != "Cancel")
            {
                var selectedIndex = Array.IndexOf(themeNames, result);
                if (selectedIndex >= 0)
                {
                    var newTheme = themes[selectedIndex];
                    ThemeChanged?.Execute(newTheme);
                }
            }
        }
    }
}