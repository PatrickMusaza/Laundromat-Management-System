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
        private string _color;

        [ObservableProperty]
        private Theme _theme;

        [ObservableProperty]
        private Language _language;

        public bool HasDescription => !string.IsNullOrEmpty(Description);
        public string PriceFormatted => Price == 0 ? "FREE" : Price.ToString("N0");

        // Colors based on service type and theme - NOW USING DATABASE COLOR
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

        public ServiceViewModel(ServiceItem item, Theme theme, Language language)
        {
            Id = item.Id;
            Name = item.Name;
            Description = item.Description;
            Price = item.Price;
            Icon = item.Icon;
            Color = item.Color; // Set color from database

            // Initialize from state service
            _theme = theme;
            _language = language;

            _cartItem = new CartItem
            {
                Id = $"{Id}-{Guid.NewGuid():N}",
                ServiceId = Id,
                Name = Name,
                Price = Price,
                Quantity = 1
            };

            // Subscribe to state changes
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
            if (_cartItem == null)
            {
                return;
            }

            try
            {
                var stateService = ApplicationStateService.Instance;
                stateService.AddToCart(_cartItem);

                // Show a quick feedback
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert(
                        GetAlertTranslation("AddedToCart"),
                        GetAlertTranslation("AddedToCartMessage"),
                        "OK");
                });
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error", $"Error adding to cart: {ex.Message}", "OK");
            }
        }

        private string GetAlertTranslation(string key)
        {
            return Language switch
            {
                Language.EN => key switch
                {
                    "AddedToCart" => "Added to Cart",
                    "AddedToCartMessage" => $"Added {Name} to cart",
                    _ => key
                },
                Language.RW => key switch
                {
                    "AddedToCart" => "Byongerewe mu gikapu",
                    "AddedToCartMessage" => $"{Name} yongerewe mu gikapu",
                    _ => key
                },
                Language.FR => key switch
                {
                    "AddedToCart" => "Ajouté au panier",
                    "AddedToCartMessage" => $"Ajouté {Name} au panier",
                    _ => key
                },
                _ => key
            };
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

        // UPDATED: Use color from database instead of hardcoded mapping
        private Color GetIconBackgroundColor()
        {
            if (!string.IsNullOrEmpty(Color))
            {
                try
                {
                    return Microsoft.Maui.Graphics.Color.FromArgb(Color);
                }
                catch
                {
                    // Fallback to hardcoded mapping if database color is invalid
                }
            }

            // Fallback to icon-based colors if no database color
            return Icon switch
            {
                "🔥" => Microsoft.Maui.Graphics.Color.FromArgb("#FEE2E2"),  // Hot water
                "💧" => Microsoft.Maui.Graphics.Color.FromArgb("#DBEAFE"),  // Cold water
                "⚡" => Microsoft.Maui.Graphics.Color.FromArgb("#FEF3C7"),  // Express wash
                "🌀" => Microsoft.Maui.Graphics.Color.FromArgb("#D1FAE5"),  // Dry cleaning
                "👔" => Microsoft.Maui.Graphics.Color.FromArgb("#E0E7FF"),  // Ironing
                "⭐" => Microsoft.Maui.Graphics.Color.FromArgb("#DCFCE7"),  // Package
                _ => Colors.White
            };
        }

        private Color GetShadowColor()
        {
            return Theme switch
            {
                Theme.Dark => Microsoft.Maui.Graphics.Color.FromArgb("#000000").WithAlpha(0.1f),
                Theme.Gray => Microsoft.Maui.Graphics.Color.FromArgb("#9CA3AF").WithAlpha(0.1f),
                _ => Microsoft.Maui.Graphics.Color.FromArgb("#000000").WithAlpha(0.1f)
            };
        }

        private Color GetIconColor()
        {
            return Icon switch
            {
                "🔥" => Microsoft.Maui.Graphics.Color.FromArgb("#EF4444"),  // Red
                "💧" => Microsoft.Maui.Graphics.Color.FromArgb("#3B82F6"),  // Blue
                "⚡" => Microsoft.Maui.Graphics.Color.FromArgb("#F59E0B"),  // Amber
                "🌀" => Microsoft.Maui.Graphics.Color.FromArgb("#10B981"),  // Green
                "👔" => Microsoft.Maui.Graphics.Color.FromArgb("#6366F1"),  // Indigo
                "⭐" => Microsoft.Maui.Graphics.Color.FromArgb("#16A34A"),  // Green
                _ => Colors.Black
            };
        }

        private Color GetServiceBackgroundColor()
        {
            return Theme switch
            {
                Theme.Dark => Microsoft.Maui.Graphics.Color.FromArgb("#1F2937"),
                Theme.Gray => Colors.White,
                _ => Colors.White
            };
        }

        private Color GetServiceBorderColor()
        {
            return Theme switch
            {
                Theme.Dark => Microsoft.Maui.Graphics.Color.FromArgb("#374151"),
                Theme.Gray => Microsoft.Maui.Graphics.Color.FromArgb("#D1D5DB"),
                _ => Microsoft.Maui.Graphics.Color.FromArgb("#E5E7EB")
            };
        }

        private Color GetTextColor()
        {
            return Theme switch
            {
                Theme.Dark => Colors.White,
                _ => Microsoft.Maui.Graphics.Color.FromArgb("#111827")
            };
        }

        private Color GetDescriptionColor()
        {
            return Theme switch
            {
                Theme.Dark => Microsoft.Maui.Graphics.Color.FromArgb("#9CA3AF"),
                _ => Microsoft.Maui.Graphics.Color.FromArgb("#6B7280")
            };
        }

        private Color GetPriceBackgroundColor()
        {
            return Price == 0 ? Microsoft.Maui.Graphics.Color.FromArgb("#D1FAE5") :
                   Microsoft.Maui.Graphics.Color.FromArgb("#F59E0B");
        }

        private Color GetTapToAddColor()
        {
            return Theme switch
            {
                Theme.Dark => Microsoft.Maui.Graphics.Color.FromArgb("#9CA3AF"),
                _ => Microsoft.Maui.Graphics.Color.FromArgb("#6B7280")
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
}