using System.Windows.Input;
using LaundromatManagementSystem.Models;
using System.Collections.ObjectModel;
using LaundromatManagementSystem.ViewModels;

namespace LaundromatManagementSystem.Views
{
    public partial class ShoppingCart : ContentView
    {
        public static readonly BindableProperty SubtotalProperty =
            BindableProperty.Create(nameof(Subtotal), typeof(decimal), typeof(ShoppingCart), 0m,
                propertyChanged: OnSubtotalChanged);

        public static readonly BindableProperty TaxProperty =
            BindableProperty.Create(nameof(Tax), typeof(decimal), typeof(ShoppingCart), 0m,
                propertyChanged: OnTaxChanged);

        public static readonly BindableProperty TotalProperty =
            BindableProperty.Create(nameof(Total), typeof(decimal), typeof(ShoppingCart), 0m,
                propertyChanged: OnTotalChanged);

        public static readonly BindableProperty LanguageProperty =
            BindableProperty.Create(nameof(Language), typeof(Language), typeof(ShoppingCart), Language.EN,
                propertyChanged: OnLanguageChanged);

        public static readonly BindableProperty ThemeProperty =
            BindableProperty.Create(nameof(Theme), typeof(Theme), typeof(ShoppingCart), Theme.Light,
                propertyChanged: OnThemeChanged);

        public decimal Subtotal
        {
            get => (decimal)GetValue(SubtotalProperty);
            set => SetValue(SubtotalProperty, value);
        }

        public decimal Tax
        {
            get => (decimal)GetValue(TaxProperty);
            set => SetValue(TaxProperty, value);
        }

        public decimal Total
        {
            get => (decimal)GetValue(TotalProperty);
            set => SetValue(TotalProperty, value);
        }

        public Language Language
        {
            get => (Language)GetValue(LanguageProperty);
            set => SetValue(LanguageProperty, value);
        }

        public Theme Theme
        {
            get => (Theme)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }

        public ShoppingCartViewModel ViewModel { get; private set; }

        public ShoppingCart()
        {
            InitializeComponent();

            // Create ViewModel with commands
            ViewModel = new ShoppingCartViewModel();

            BindingContext = ViewModel;

            // Set initial values
            ViewModel.Subtotal = Subtotal;
            ViewModel.Tax = Tax;
            ViewModel.Total = Total;
            ViewModel.Language = Language;
            ViewModel.Theme = Theme;
        }

        private static void OnSubtotalChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ShoppingCart shoppingCart && newValue is decimal subtotal)
            {
                shoppingCart.ViewModel.Subtotal = subtotal;
            }
        }

        private static void OnTaxChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ShoppingCart shoppingCart && newValue is decimal tax)
            {
                shoppingCart.ViewModel.Tax = tax;
            }
        }

        private static void OnTotalChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ShoppingCart shoppingCart && newValue is decimal total)
            {
                shoppingCart.ViewModel.Total = total;
            }
        }

        private static void OnLanguageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ShoppingCart shoppingCart && newValue is Language language)
            {
                shoppingCart.ViewModel.Language = language;
            }
        }

        private static void OnThemeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ShoppingCart shoppingCart && newValue is Theme theme)
            {
                shoppingCart.ViewModel.Theme = theme;
            }
        }
    }
}