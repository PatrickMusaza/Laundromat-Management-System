using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaundromatManagementSystem.Models;
using LaundromatManagementSystem.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LaundromatManagementSystem.ViewModels
{
    public partial class ServiceGridViewModel : ObservableObject
    {
        private readonly IServiceService _serviceService;
        private readonly ApplicationStateService _stateService = ApplicationStateService.Instance;

        [ObservableProperty]
        private string _selectedCategory = "washing";

        // Keep settable properties for XAML binding compatibility
        [ObservableProperty]
        private Language _language;

        [ObservableProperty]
        private Theme _theme;

        [ObservableProperty]
        private ObservableCollection<ServiceViewModel> _services = new();

        private readonly Action<CartItem> _addToCart;

        // Text properties
        public string WashText => GetCategoryText("WASH");
        public string DryText => GetCategoryText("DRY");
        public string AddonText => GetCategoryText("ADD-ON");
        public string PackageText => GetCategoryText("PACKAGE");

        // Button colors based on selection and theme
        public Color WashButtonBackground => GetButtonBackground("washing");
        public Color WashButtonBorder => GetButtonBorder("washing");
        public Color WashButtonTextColor => GetButtonTextColor("washing");

        public Color DryButtonBackground => GetButtonBackground("drying");
        public Color DryButtonBorder => GetButtonBorder("drying");
        public Color DryButtonTextColor => GetButtonTextColor("drying");

        public Color AddonButtonBackground => GetButtonBackground("addon");
        public Color AddonButtonBorder => GetButtonBorder("addon");
        public Color AddonButtonTextColor => GetButtonTextColor("addon");

        public Color PackageButtonBackground => GetButtonBackground("package");
        public Color PackageButtonBorder => GetButtonBorder("package");
        public Color PackageButtonTextColor => GetButtonTextColor("package");

        public ICommand CategoryChangedCommand { get; }
        public ICommand AddToCartCommand { get; }

        public ServiceGridViewModel(IServiceService serviceService,
                                   ICommand categoryChangedCommand,
                                   Action<CartItem> addToCart)
        {
            _serviceService = serviceService;
            CategoryChangedCommand = categoryChangedCommand;
            _addToCart = addToCart;

            // Initialize from state service
            _language = _stateService.CurrentLanguage;
            _theme = _stateService.CurrentTheme;

            // Subscribe to state changes
            _stateService.PropertyChanged += OnStateChanged;

            LoadServices();
        }

        // Override setters to update state service
        partial void OnLanguageChanged(Language value)
        {
            if (_stateService.CurrentLanguage != value)
            {
                _stateService.CurrentLanguage = value;
            }
            UpdateAllLanguageProperties();
        }

        partial void OnThemeChanged(Theme value)
        {
            if (_stateService.CurrentTheme != value)
            {
                _stateService.CurrentTheme = value;
            }
            UpdateAllThemeProperties();
        }

        private void UpdateAllLanguageProperties()
        {
            OnPropertyChanged(nameof(WashText));
            OnPropertyChanged(nameof(DryText));
            OnPropertyChanged(nameof(AddonText));
            OnPropertyChanged(nameof(PackageText));

            foreach (var service in Services)
            {
                service.Language = Language;
            }
        }

        private void UpdateAllThemeProperties()
        {
            RaiseCategoryButtonColors();
            foreach (var service in Services)
            {
                service.Theme = Theme;
            }
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

        partial void OnSelectedCategoryChanged(string value)
        {
            CategoryChangedCommand?.Execute(value);
            LoadServices();
            RaiseCategoryButtonColors();
        }

        private void RaiseCategoryButtonColors()
        {
            OnPropertyChanged(nameof(WashButtonBackground));
            OnPropertyChanged(nameof(WashButtonBorder));
            OnPropertyChanged(nameof(WashButtonTextColor));
            OnPropertyChanged(nameof(DryButtonBackground));
            OnPropertyChanged(nameof(DryButtonBorder));
            OnPropertyChanged(nameof(DryButtonTextColor));
            OnPropertyChanged(nameof(AddonButtonBackground));
            OnPropertyChanged(nameof(AddonButtonBorder));
            OnPropertyChanged(nameof(AddonButtonTextColor));
            OnPropertyChanged(nameof(PackageButtonBackground));
            OnPropertyChanged(nameof(PackageButtonBorder));
            OnPropertyChanged(nameof(PackageButtonTextColor));
        }

        [RelayCommand]
        private void SelectWash() => SelectedCategory = "washing";

        [RelayCommand]
        private void SelectDry() => SelectedCategory = "drying";

        [RelayCommand]
        private void SelectAddon() => SelectedCategory = "addon";

        [RelayCommand]
        private void SelectPackage() => SelectedCategory = "package";

        private async void LoadServices()
        {
            var serviceItems = await _serviceService.GetServicesByCategoryAsync(SelectedCategory, Language);
            Services.Clear();

            foreach (var item in serviceItems)
            {
                Services.Add(new ServiceViewModel(item, _addToCart, Theme, Language));
            }
        }

        private string GetCategoryText(string categoryKey)
        {
            var translations = new Dictionary<string, Dictionary<string, string>>
            {
                ["WASH"] = new()
                {
                    ["EN"] = "WASH",
                    ["RW"] = "KARABA",
                    ["FR"] = "LAVER"
                },
                ["DRY"] = new()
                {
                    ["EN"] = "DRY",
                    ["RW"] = "UMISHA",
                    ["FR"] = "SÉCHER"
                },
                ["ADD-ON"] = new()
                {
                    ["EN"] = "ADD-ON",
                    ["RW"] = "ONGERAHO",
                    ["FR"] = "SUPPLÉMENT"
                },
                ["PACKAGE"] = new()
                {
                    ["EN"] = "PACKAGE",
                    ["RW"] = "PAKI",
                    ["FR"] = "FORFAIT"
                }
            };

            return translations[categoryKey][Language.ToString()];
        }

        private Color GetButtonBackground(string category)
        {
            if (SelectedCategory == category)
                return Color.FromArgb("#1E3A8A");

            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#1F2937"),
                Theme.Gray => Colors.White,
                _ => Colors.White
            };
        }

        private Color GetButtonBorder(string category)
        {
            if (SelectedCategory == category)
                return Colors.Transparent;

            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#374151"),
                Theme.Gray => Color.FromArgb("#D1D5DB"),
                _ => Color.FromArgb("#E5E7EB")
            };
        }

        private Color GetButtonTextColor(string category)
        {
            if (SelectedCategory == category)
                return Colors.White;

            return Theme switch
            {
                Theme.Dark => Colors.White,
                Theme.Gray => Color.FromArgb("#6B7280"),
                _ => Color.FromArgb("#6B7280")
            };
        }

        // Clean up
        public void Cleanup()
        {
            _stateService.PropertyChanged -= OnStateChanged;
        }
    }
}