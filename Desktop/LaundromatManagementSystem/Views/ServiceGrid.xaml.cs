using System.Windows.Input;
using LaundromatManagementSystem.Services;
using LaundromatManagementSystem.ViewModels;

namespace LaundromatManagementSystem.Views
{
    public partial class ServiceGrid : ContentView
    {
        public static readonly BindableProperty SelectedCategoryProperty =
            BindableProperty.Create(nameof(SelectedCategory), typeof(string), typeof(ServiceGrid), "washing",
                propertyChanged: OnSelectedCategoryChanged);

        public static readonly BindableProperty LanguageProperty =
            BindableProperty.Create(nameof(Language), typeof(Language), typeof(ServiceGrid), Language.EN,
                propertyChanged: OnLanguageChanged);

        public static readonly BindableProperty ThemeProperty =
            BindableProperty.Create(nameof(Theme), typeof(Theme), typeof(ServiceGrid), Theme.Light,
                propertyChanged: OnThemeChanged);

        public static readonly BindableProperty CategoryChangedCommandProperty =
            BindableProperty.Create(nameof(CategoryChangedCommand), typeof(ICommand), typeof(ServiceGrid));

        public static readonly BindableProperty AddToCartCommandProperty =
            BindableProperty.Create(nameof(AddToCartCommand), typeof(ICommand), typeof(ServiceGrid));

        public string SelectedCategory
        {
            get => (string)GetValue(SelectedCategoryProperty);
            set => SetValue(SelectedCategoryProperty, value);
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

        public ICommand CategoryChangedCommand
        {
            get => (ICommand)GetValue(CategoryChangedCommandProperty);
            set => SetValue(CategoryChangedCommandProperty, value);
        }

        public ICommand AddToCartCommand
        {
            get => (ICommand)GetValue(AddToCartCommandProperty);
            set => SetValue(AddToCartCommandProperty, value);
        }

        public ServiceGridViewModel ViewModel { get; private set; }

        public ServiceGrid()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (ViewModel != null)
                return;

            var serviceService = ServiceLocator.GetService<IServiceService>();

            ViewModel = new ServiceGridViewModel(
                serviceService,
                CategoryChangedCommand
            );

            ViewModel.SelectedCategory = SelectedCategory;
            ViewModel.Language = Language;
            ViewModel.Theme = Theme;

            BindingContext = ViewModel;
        }

        private static void OnSelectedCategoryChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ServiceGrid serviceGrid && newValue is string category)
            {
                serviceGrid.ViewModel.SelectedCategory = category;
            }
        }

        private static void OnLanguageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ServiceGrid serviceGrid && newValue is Language language)
            {
                serviceGrid.ViewModel.Language = language;
            }
        }

        private static void OnThemeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ServiceGrid serviceGrid && newValue is Theme theme)
            {
                serviceGrid.ViewModel.Theme = theme;
            }
        }
    }
}