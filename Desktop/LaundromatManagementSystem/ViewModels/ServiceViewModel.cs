using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaundromatManagementSystem.Models;
using LaundromatManagementSystem.Services;

namespace LaundromatManagementSystem.ViewModels
{
    public partial class ServiceViewModel : ObservableObject
    {
        private readonly ApplicationStateService _stateService = ApplicationStateService.Instance;

        [ObservableProperty]
        private string _id;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private decimal _price;

        [ObservableProperty]
        private string _icon;

        [ObservableProperty]
        private Theme _theme;

        [ObservableProperty]
        private Language _language;

        private readonly Action<CartItem> _addToCart;

        public bool HasDescription => !string.IsNullOrEmpty(Description);
        public string PriceFormatted => Price == 0 ? "FREE" : Price.ToString("N0");

        // Colors based on service type and theme
        public Color IconBackgroundColor => GetIconBackgroundColor();
        public Color ShadowColor => GetShadowColor();
        public Color IconColor => GetIconColor();
        public Color ServiceBackgroundColor => GetServiceBackgroundColor();
        public Color ServiceBorderColor => GetServiceBorderColor();
        public Color TextColor => GetTextColor();
        public Color DescriptionColor => GetDescriptionColor();
        public Color PriceBackgroundColor => GetPriceBackgroundColor();
        public Color TapToAddColor => GetTapToAddColor();
        public string TapToAddText => GetTapToAddText();

        private CartItem _cartItem;

        public ServiceViewModel(ServiceItem item, Action<CartItem> addToCart, Theme theme, Language language)
        {
            Id = item.Id;
            Name = item.Name;
            Description = item.Description;
            Price = item.Price;
            Icon = item.Icon;

            // Initialize from state service
            _theme = theme;
            _language = language;

            _addToCart = addToCart;

            _cartItem = new CartItem
            {
                Id = $"{Id}-{Guid.NewGuid():N}",
                Name = Name,
                Price = Price,
                Quantity = 1
            };

            // Subscribe to state changes (if this service needs to update dynamically)
            _stateService.PropertyChanged += OnStateChanged;
        }

        // Override setters to update state service
        partial void OnThemeChanged(Theme value)
        {
            if (_stateService.CurrentTheme != value)
            {
                _stateService.CurrentTheme = value;
            }
            UpdateThemeProperties();
        }

        partial void OnLanguageChanged(Language value)
        {
            if (_stateService.CurrentLanguage != value)
            {
                _stateService.CurrentLanguage = value;
            }
            UpdateLanguageProperties();
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

        [RelayCommand]
        private void AddToCart()
        {
            if (_cartItem != null)
            {
                _addToCart?.Invoke(_cartItem);
            }
        }

        private void UpdateThemeProperties()
        {
            OnPropertyChanged(nameof(IconBackgroundColor));
            OnPropertyChanged(nameof(IconColor));
            OnPropertyChanged(nameof(ServiceBackgroundColor));
            OnPropertyChanged(nameof(ServiceBorderColor));
            OnPropertyChanged(nameof(TextColor));
            OnPropertyChanged(nameof(DescriptionColor));
            OnPropertyChanged(nameof(PriceBackgroundColor));
            OnPropertyChanged(nameof(TapToAddColor));
        }

        private void UpdateLanguageProperties()
        {
            OnPropertyChanged(nameof(TapToAddText));
        }

        private Color GetIconBackgroundColor()
        {
            return Icon switch
            {
                "🔥" => Color.FromArgb("#FEE2E2"),  // Hot water
                "💧" => Color.FromArgb("#DBEAFE"),  // Cold water
                "⚡" => Color.FromArgb("#FEF3C7"),  // Express wash
                "🌀" => Color.FromArgb("#D1FAE5"),  // Dry cleaning
                "👔" => Color.FromArgb("#E0E7FF"),  // Ironing
                "⭐" => Color.FromArgb("#DCFCE7"),  // Package
                _ => Colors.White
            };
        }

        private Color GetShadowColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#000000").WithAlpha(0.1f),
                Theme.Gray => Color.FromArgb("#9CA3AF").WithAlpha(0.1f),
                _ => Color.FromArgb("#000000").WithAlpha(0.1f)
            };
        }

        private Color GetIconColor()
        {
            return Icon switch
            {
                "🔥" => Color.FromArgb("#EF4444"),  // Red
                "💧" => Color.FromArgb("#3B82F6"),  // Blue
                "⚡" => Color.FromArgb("#F59E0B"),  // Amber
                "🌀" => Color.FromArgb("#10B981"),  // Green
                "👔" => Color.FromArgb("#6366F1"),  // Indigo
                "⭐" => Color.FromArgb("#16A34A"),  // Green
                _ => Colors.Black
            };
        }

        private Color GetServiceBackgroundColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#1F2937"),
                Theme.Gray => Colors.White,
                _ => Colors.White
            };
        }

        private Color GetServiceBorderColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#374151"),
                Theme.Gray => Color.FromArgb("#D1D5DB"),
                _ => Color.FromArgb("#E5E7EB")
            };
        }

        private Color GetTextColor()
        {
            return Theme switch
            {
                Theme.Dark => Colors.White,
                _ => Color.FromArgb("#111827")
            };
        }

        private Color GetDescriptionColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#9CA3AF"),
                _ => Color.FromArgb("#6B7280")
            };
        }

        private Color GetPriceBackgroundColor()
        {
            return Price == 0 ? Color.FromArgb("#D1FAE5") :
                   Color.FromArgb("#F59E0B");
        }

        private Color GetTapToAddColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#9CA3AF"),
                _ => Color.FromArgb("#6B7280")
            };
        }

        private string GetTapToAddText()
        {
            return Language switch
            {
                Language.EN => "Tap to add",
                Language.RW => "Kanda kongeraho",
                Language.FR => "Appuyez pour ajouter",
                _ => "Tap to add"
            };
        }

        // Clean up subscriptions
        public void Cleanup()
        {
            _stateService.PropertyChanged -= OnStateChanged;
        }
    }

    public class ServiceItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}