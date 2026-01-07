using System.Windows.Input;
using LaundromatManagementSystem.ViewModels;

namespace LaundromatManagementSystem.Views
{
    public partial class CustomerHeader : ContentView
    {
        public static readonly BindableProperty LanguageProperty =
            BindableProperty.Create(nameof(Language), typeof(Language), typeof(CustomerHeader), Language.EN,
                propertyChanged: OnLanguageChanged);

        public static readonly BindableProperty ThemeProperty =
            BindableProperty.Create(nameof(Theme), typeof(Theme), typeof(CustomerHeader), Theme.Light,
                propertyChanged: OnThemeChanged);

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

        public CustomerHeaderViewModel ViewModel { get; private set; }

        public CustomerHeader()
        {
            InitializeComponent();

            ViewModel = new CustomerHeaderViewModel();

            BindingContext = ViewModel;
        }

        private static void OnLanguageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomerHeader header && newValue is Language language)
            {
                header.ViewModel.Language = language;
            }
        }

        private static void OnThemeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomerHeader header && newValue is Theme theme)
            {
                header.ViewModel.Theme = theme;
            }
        }
    }
}