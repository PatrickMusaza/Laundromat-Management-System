using System.Windows.Input;
using LaundromatManagementSystem.ViewModels;

namespace LaundromatManagementSystem.Views
{
    public partial class PaymentModal : ContentView
    {
        public static readonly BindableProperty TotalProperty =
            BindableProperty.Create(nameof(Total), typeof(decimal), typeof(PaymentModal), 0m,
                propertyChanged: OnTotalChanged);
        
        public static readonly BindableProperty TransactionIdProperty =
            BindableProperty.Create(nameof(TransactionId), typeof(string), typeof(PaymentModal), string.Empty,
                propertyChanged: OnTransactionIdChanged);
        
        public static readonly BindableProperty LanguageProperty =
            BindableProperty.Create(nameof(Language), typeof(Language), typeof(PaymentModal), Language.EN,
                propertyChanged: OnLanguageChanged);
        
        public static readonly BindableProperty ThemeProperty =
            BindableProperty.Create(nameof(Theme), typeof(Theme), typeof(PaymentModal), Theme.Light,
                propertyChanged: OnThemeChanged);
        
        public static readonly BindableProperty CloseCommandProperty =
            BindableProperty.Create(nameof(CloseCommand), typeof(ICommand), typeof(PaymentModal));
        
        public static readonly BindableProperty PaymentCompleteCommandProperty =
            BindableProperty.Create(nameof(PaymentCompleteCommand), typeof(ICommand), typeof(PaymentModal));
        
        public decimal Total
        {
            get => (decimal)GetValue(TotalProperty);
            set => SetValue(TotalProperty, value);
        }
        
        public string TransactionId
        {
            get => (string)GetValue(TransactionIdProperty);
            set => SetValue(TransactionIdProperty, value);
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
        
        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }
        
        public ICommand PaymentCompleteCommand
        {
            get => (ICommand)GetValue(PaymentCompleteCommandProperty);
            set => SetValue(PaymentCompleteCommandProperty, value);
        }
        
        public PaymentModalViewModel ViewModel { get; private set; }
        
        public PaymentModal()
        {
            InitializeComponent();
            
            // Create ViewModel with commands
            ViewModel = new PaymentModalViewModel(
                CloseCommand,
                PaymentCompleteCommand
            );
            
            BindingContext = ViewModel;
            
            // Set initial values
            ViewModel.Total = Total;
            ViewModel.TransactionId = TransactionId;
            ViewModel.Language = Language;
            ViewModel.Theme = Theme;
        }
        
        private static void OnTotalChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is PaymentModal paymentModal && newValue is decimal total)
            {
                paymentModal.ViewModel.Total = total;
            }
        }
        
        private static void OnTransactionIdChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is PaymentModal paymentModal && newValue is string transactionId)
            {
                paymentModal.ViewModel.TransactionId = transactionId;
            }
        }
        
        private static void OnLanguageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is PaymentModal paymentModal && newValue is Language language)
            {
                paymentModal.ViewModel.Language = language;
            }
        }
        
        private static void OnThemeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is PaymentModal paymentModal && newValue is Theme theme)
            {
                paymentModal.ViewModel.Theme = theme;
            }
        }
    }
}