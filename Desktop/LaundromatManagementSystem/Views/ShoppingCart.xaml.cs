using System.Windows.Input;
using LaundromatManagementSystem.Models;
using System.Collections.ObjectModel;
using LaundromatManagementSystem.ViewModels;

namespace LaundromatManagementSystem.Views
{
    public partial class ShoppingCart : ContentView
    {
        public static readonly BindableProperty CartProperty =
            BindableProperty.Create(nameof(Cart), typeof(ObservableCollection<CartItem>), typeof(ShoppingCart), 
                new ObservableCollection<CartItem>(),
                propertyChanged: OnCartChanged);
        
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
        
        public static readonly BindableProperty RemoveItemCommandProperty =
            BindableProperty.Create(nameof(RemoveItemCommand), typeof(ICommand), typeof(ShoppingCart));
        
        public static readonly BindableProperty UpdateQuantityCommandProperty =
            BindableProperty.Create(nameof(UpdateQuantityCommand), typeof(ICommand), typeof(ShoppingCart));
        
        public static readonly BindableProperty ProcessPaymentCommandProperty =
            BindableProperty.Create(nameof(ProcessPaymentCommand), typeof(ICommand), typeof(ShoppingCart));
        
        public ObservableCollection<CartItem> Cart
        {
            get => (ObservableCollection<CartItem>)GetValue(CartProperty);
            set => SetValue(CartProperty, value);
        }
        
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
        
        public ICommand RemoveItemCommand
        {
            get => (ICommand)GetValue(RemoveItemCommandProperty);
            set => SetValue(RemoveItemCommandProperty, value);
        }
        
        public ICommand UpdateQuantityCommand
        {
            get => (ICommand)GetValue(UpdateQuantityCommandProperty);
            set => SetValue(UpdateQuantityCommandProperty, value);
        }
        
        public ICommand ProcessPaymentCommand
        {
            get => (ICommand)GetValue(ProcessPaymentCommandProperty);
            set => SetValue(ProcessPaymentCommandProperty, value);
        }
        
        public ShoppingCartViewModel ViewModel { get; private set; }
        
        public ShoppingCart()
        {
            InitializeComponent();
            
            // Create ViewModel with commands
            ViewModel = new ShoppingCartViewModel(
                RemoveItemCommand,
                UpdateQuantityCommand,
                ProcessPaymentCommand
            );
            
            BindingContext = ViewModel;
            
            // Set initial values
            ViewModel.Subtotal = Subtotal;
            ViewModel.Tax = Tax;
            ViewModel.Total = Total;
            ViewModel.Language = Language;
            ViewModel.Theme = Theme;
        }
        
        private static void OnCartChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ShoppingCart shoppingCart && newValue is ObservableCollection<CartItem> cart)
            {
                // shoppingCart.ViewModel.Cart = cart; // Updated to use a method to set the cart
            }
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