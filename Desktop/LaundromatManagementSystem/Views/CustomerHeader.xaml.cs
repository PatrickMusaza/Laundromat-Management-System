using System.Windows.Input;

namespace LaundromatManagementSystem.Views
{
    public partial class CustomerHeader : ContentView
    {
        public static readonly BindableProperty LanguageProperty =
            BindableProperty.Create(nameof(Language), typeof(string), typeof(CustomerHeader), "EN",
                propertyChanged: OnLanguageChanged);

        public static readonly BindableProperty ThemeProperty =
            BindableProperty.Create(nameof(Theme), typeof(string), typeof(CustomerHeader), "dark",
                propertyChanged: OnThemeChanged);

        public static readonly BindableProperty LanguageChangedCommandProperty =
            BindableProperty.Create(nameof(LanguageChangedCommand), typeof(ICommand), typeof(CustomerHeader));

        public static readonly BindableProperty ThemeChangedCommandProperty =
            BindableProperty.Create(nameof(ThemeChangedCommand), typeof(ICommand), typeof(CustomerHeader));

        public static readonly BindableProperty LoginCommandProperty =
                    BindableProperty.Create(nameof(LoginCommand), typeof(ICommand), typeof(CustomerHeader));

        public static readonly BindableProperty LoginProperty =
            BindableProperty.Create(nameof(Login), typeof(string), typeof(CustomerHeader), "Login");

        public static readonly BindableProperty CurrentUserProperty =
            BindableProperty.Create(nameof(CurrentUser), typeof(string), typeof(CustomerHeader), "Guest");

            public static readonly BindableProperty CurrentUserCommandProperty =
            BindableProperty.Create(nameof(CurrentUserCommand), typeof(ICommand), typeof(CustomerHeader));

        // Properties with backing fields
        private string _welcomeText = "Welcome to Laundromat";
        private string _selectServiceText = "Select Your Services";
        private string _languageLabelText = "Language";
        private string _themeLabelText = "Appearance";
        private string _currentThemeText = "Dark";
        private string _themeIcon = "🌙";
        private string _loginLabelText = "Login";
        private string _currentLoginText = "Guest";

        public string WelcomeText
        {
            get => _welcomeText;
            set
            {
                if (_welcomeText != value)
                {
                    _welcomeText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LoginLabelText
        {
            get => _loginLabelText;
            set
            {
                if (_loginLabelText != value)
                {
                    _loginLabelText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CurrentLoggedInUser
        {
            get => _currentLoginText;
            set
            {
                if (_currentLoginText != value)
                {
                    _currentLoginText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SelectServiceText
        {
            get => _selectServiceText;
            set
            {
                if (_selectServiceText != value)
                {
                    _selectServiceText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LanguageLabelText
        {
            get => _languageLabelText;
            set
            {
                if (_languageLabelText != value)
                {
                    _languageLabelText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ThemeLabelText
        {
            get => _themeLabelText;
            set
            {
                if (_themeLabelText != value)
                {
                    _themeLabelText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CurrentThemeText
        {
            get => _currentThemeText;
            set
            {
                if (_currentThemeText != value)
                {
                    _currentThemeText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LoginText
        {
            get => _loginLabelText;
            set
            {
                if (_loginLabelText != value)
                {
                    _loginLabelText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CurrentUser
        {
            get => _currentLoginText;
            set
            {
                if (_currentLoginText != value)
                {
                    _currentLoginText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ThemeIcon
        {
            get => _themeIcon;
            set
            {
                if (_themeIcon != value)
                {
                    _themeIcon = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public string Login
        {
            get => (string)GetValue(LoginProperty);
            set => SetValue(LoginProperty, value);
        }

        public ICommand LanguageChangedCommand
        {
            get => (ICommand)GetValue(LanguageChangedCommandProperty);
            set => SetValue(LanguageChangedCommandProperty, value);
        }

        public ICommand ThemeChangedCommand
        {
            get => (ICommand)GetValue(ThemeChangedCommandProperty);
            set => SetValue(ThemeChangedCommandProperty, value);
        }

        public ICommand LoginCommand
        {
            get => (ICommand)GetValue(LoginCommandProperty);
            set => SetValue(LoginCommandProperty, value);
        }

        public ICommand CurrentUserCommand
        {
            get => (ICommand)GetValue(CurrentUserCommandProperty);
            set => SetValue(CurrentUserCommandProperty, value);
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
                ["dark"] = "Dark",
                ["login"] = "Login",
                ["currentUser"] = "Guest"
            },
            ["RW"] = new()
            {
                ["welcome"] = "Murakaza Neza",
                ["selectService"] = "Hitamo Serivisi Zawe",
                ["language"] = "Ururimi",
                ["theme"] = "Isura",
                ["light"] = "Yera",
                ["gray"] = "Icyatsi",
                ["dark"] = "Umwijima",
                ["login"] = "Injira",
                ["currentUser"] = "Umushyitsi"
            },
            ["FR"] = new()
            {
                ["welcome"] = "Bienvenue à la Laverie",
                ["selectService"] = "Sélectionnez Vos Services",
                ["language"] = "Langue",
                ["theme"] = "Apparence",
                ["light"] = "Clair",
                ["gray"] = "Gris",
                ["dark"] = "Sombre",
                ["login"] = "Connexion",
                ["currentUser"] = "Invité"
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
            BindingContext = this;
            UpdateTranslations();
        }

        private static void OnLanguageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomerHeader header)
            {
                header.UpdateTranslations();
            }
        }

        private static void OnThemeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomerHeader header)
            {
                header.UpdateTranslations();
            }
        }

        private void UpdateTranslations()
        {
            var t = _translations[Language];

            WelcomeText = t["welcome"];
            SelectServiceText = t["selectService"];
            LanguageLabelText = t["language"];
            ThemeLabelText = t["theme"];
            CurrentThemeText = t[Theme];
            ThemeIcon = _themeIcons[Theme];

            OnPropertyChanged(nameof(Language));
        }

        public async void OnLanguageTapped(object sender, System.EventArgs e)
        {
            var languages = new[] { "EN", "RW", "FR" };
            var languageNames = new[] { "English", "Kinyarwanda", "Français" };

            // Get the current page from the Parent
            var page = GetParentPage();
            if (page != null)
            {
                var result = await page.DisplayActionSheet(
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

                        // Execute the command
                        if (LanguageChangedCommand != null && LanguageChangedCommand.CanExecute(newLanguage))
                        {
                            LanguageChangedCommand.Execute(newLanguage);
                        }
                        else
                        {
                            // Fallback: Update the language directly if command fails
                            Language = newLanguage;
                        }
                    }
                }
            }
        }

        public async void OnThemeTapped(object sender, System.EventArgs e)
        {
            var t = _translations[Language];
            var themes = new[] { "light", "gray", "dark" };
            var themeNames = new[] { t["light"], t["gray"], t["dark"] };

            // Get the current page from the Parent
            var page = GetParentPage();
            if (page != null)
            {
                var result = await page.DisplayActionSheet(
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

                        // Execute the command
                        if (ThemeChangedCommand != null && ThemeChangedCommand.CanExecute(newTheme))
                        {
                            ThemeChangedCommand.Execute(newTheme);
                        }
                        else
                        {
                            // Fallback: Update the theme directly if command fails
                            Theme = newTheme;
                        }
                    }
                }
            }
        }

        // Helper method to find the parent Page
        private Page? GetParentPage()
        {
            var parent = this.Parent;
            while (parent != null)
            {
                if (parent is Page page)
                {
                    return page;
                }
                parent = parent.Parent;
            }
            return null;
        }
    }
}