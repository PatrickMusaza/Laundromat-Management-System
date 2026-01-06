using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaundromatManagementSystem.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LaundromatManagementSystem.ViewModels
{
    public partial class ShoppingCartViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<CartItem> _cart = new();
        
        [ObservableProperty]
        private decimal _subtotal;
        
        [ObservableProperty]
        private decimal _tax;
        
        [ObservableProperty]
        private decimal _total;
        
        [ObservableProperty]
        private Language _language = Language.EN;
        
        [ObservableProperty]
        private Theme _theme = Theme.Light;
        
        // Text properties
        public string CartTitle => GetTranslation("cart");
        public string EmptyCartText => GetTranslation("empty");
        public string EmptyMessageText => GetTranslation("emptyMsg");
        public string QuantityText => GetTranslation("quantity");
        public string SubtotalText => GetTranslation("subtotal");
        public string TaxText => GetTranslation("tax");
        public string TotalText => GetTranslation("total");
        public string ProcessPaymentText => GetTranslation("process");
        
        // Theme colors
        public Color CartBackgroundColor => GetCartBackgroundColor();
        public Color CartBorderColor => GetCartBorderColor();
        public Color CartTitleColor => GetCartTitleColor();
        public Color EmptyTextColor => GetEmptyTextColor();
        public Color EmptyMessageColor => GetEmptyMessageColor();
        public Color ItemBackgroundColor => GetItemBackgroundColor();
        public Color ItemBorderColor => GetItemBorderColor();
        public Color ItemTextColor => GetItemTextColor();
        public Color TotalBackgroundColor => GetTotalBackgroundColor();
        public Color TotalBorderColor => GetTotalBorderColor();
        
        public ICommand RemoveItemCommand { get; }
        public ICommand UpdateQuantityCommand { get; }
        public ICommand ProcessPaymentCommand { get; }
        
        public ShoppingCartViewModel(ICommand removeItemCommand, 
                                    ICommand updateQuantityCommand, 
                                    ICommand processPaymentCommand)
        {
            RemoveItemCommand = removeItemCommand;
            UpdateQuantityCommand = updateQuantityCommand;
            ProcessPaymentCommand = processPaymentCommand;
        }
        
        partial void OnCartChanged(ObservableCollection<CartItem> value)
        {
            CalculateTotals();
            OnPropertyChanged(nameof(CartTitle));
            OnPropertyChanged(nameof(EmptyCartText));
        }
        
        partial void OnLanguageChanged(Language value)
        {
            OnPropertyChanged(nameof(CartTitle));
            OnPropertyChanged(nameof(EmptyCartText));
            OnPropertyChanged(nameof(EmptyMessageText));
            OnPropertyChanged(nameof(QuantityText));
            OnPropertyChanged(nameof(SubtotalText));
            OnPropertyChanged(nameof(TaxText));
            OnPropertyChanged(nameof(TotalText));
            OnPropertyChanged(nameof(ProcessPaymentText));
        }
        
        partial void OnThemeChanged(Theme value)
        {
            OnPropertyChanged(nameof(CartBackgroundColor));
            OnPropertyChanged(nameof(CartBorderColor));
            OnPropertyChanged(nameof(CartTitleColor));
            OnPropertyChanged(nameof(EmptyTextColor));
            OnPropertyChanged(nameof(EmptyMessageColor));
            OnPropertyChanged(nameof(ItemBackgroundColor));
            OnPropertyChanged(nameof(ItemBorderColor));
            OnPropertyChanged(nameof(ItemTextColor));
            OnPropertyChanged(nameof(TotalBackgroundColor));
            OnPropertyChanged(nameof(TotalBorderColor));
        }
        
        private void CalculateTotals()
        {
            Subtotal = Cart.Sum(item => item.TotalPrice);
            Tax = Math.Round(Subtotal * 0.1m, 2);
            Total = Subtotal + Tax;
        }
        
        private string GetTranslation(string key)
        {
            var translations = new Dictionary<string, Dictionary<Language, string>>
            {
                ["cart"] = new()
                {
                    [Language.EN] = "Shopping Cart",
                    [Language.RW] = "Igitebo",
                    [Language.FR] = "Panier"
                },
                ["empty"] = new()
                {
                    [Language.EN] = "Your cart is empty",
                    [Language.RW] = "Igitebo cyawe kirimo ubusa",
                    [Language.FR] = "Votre panier est vide"
                },
                ["emptyMsg"] = new()
                {
                    [Language.EN] = "Add services to get started",
                    [Language.RW] = "Ongeraho serivisi",
                    [Language.FR] = "Ajoutez des services pour commencer"
                },
                ["quantity"] = new()
                {
                    [Language.EN] = "Qty",
                    [Language.RW] = "Umubare",
                    [Language.FR] = "Qté"
                },
                ["subtotal"] = new()
                {
                    [Language.EN] = "Subtotal",
                    [Language.RW] = "Igiteranyo",
                    [Language.FR] = "Sous-total"
                },
                ["tax"] = new()
                {
                    [Language.EN] = "Tax (10%)",
                    [Language.RW] = "Umusoro (10%)",
                    [Language.FR] = "Taxe (10%)"
                },
                ["total"] = new()
                {
                    [Language.EN] = "TOTAL",
                    [Language.RW] = "IGITERANYO",
                    [Language.FR] = "TOTAL"
                },
                ["process"] = new()
                {
                    [Language.EN] = "PROCESS PAYMENT",
                    [Language.RW] = "KWISHYURA",
                    [Language.FR] = "TRAITER LE PAIEMENT"
                }
            };
            
            return translations[key][Language];
        }
        
        private Color GetCartBackgroundColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#1F2937"),
                Theme.Gray => Colors.White,
                _ => Colors.White
            };
        }
        
        private Color GetCartBorderColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#374151"),
                Theme.Gray => Color.FromArgb("#D1D5DB"),
                _ => Color.FromArgb("#E5E7EB")
            };
        }
        
        private Color GetCartTitleColor()
        {
            return Theme switch
            {
                Theme.Dark => Colors.White,
                _ => Color.FromArgb("#1E3A8A")
            };
        }
        
        private Color GetEmptyTextColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#9CA3AF"),
                _ => Color.FromArgb("#6B7280")
            };
        }
        
        private Color GetEmptyMessageColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#6B7280"),
                _ => Color.FromArgb("#9CA3AF")
            };
        }
        
        private Color GetItemBackgroundColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#111827"),
                Theme.Gray => Color.FromArgb("#F3F4F6"),
                _ => Color.FromArgb("#F9FAFB")
            };
        }
        
        private Color GetItemBorderColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#374151"),
                Theme.Gray => Color.FromArgb("#D1D5DB"),
                _ => Color.FromArgb("#E5E7EB")
            };
        }
        
        private Color GetItemTextColor()
        {
            return Theme switch
            {
                Theme.Dark => Colors.White,
                _ => Color.FromArgb("#111827")
            };
        }
        
        private Color GetTotalBackgroundColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#111827"),
                Theme.Gray => Color.FromArgb("#F3F4F6"),
                _ => Color.FromArgb("#F9FAFB")
            };
        }
        
        private Color GetTotalBorderColor()
        {
            return Theme switch
            {
                Theme.Dark => Color.FromArgb("#374151"),
                Theme.Gray => Color.FromArgb("#D1D5DB"),
                _ => Color.FromArgb("#E5E7EB")
            };
        }
    }
}