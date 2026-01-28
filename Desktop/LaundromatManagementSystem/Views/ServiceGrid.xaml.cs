using System.Windows.Input;
using LaundromatManagementSystem.Services;
using LaundromatManagementSystem.ViewModels;
using LaundromatManagementSystem.Models;

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

        private ServiceGridViewModel _viewModel;

        public ServiceGrid()
        {
            InitializeComponent();
            InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            try
            {
                var serviceService = GetServiceService();
                
                _viewModel = new ServiceGridViewModel(
                    serviceService,
                    CategoryChangedCommand
                );

                _viewModel.SelectedCategory = SelectedCategory;
                _viewModel.Language = Language;
                _viewModel.Theme = Theme;

                BindingContext = _viewModel;
            }
            catch (Exception ex)
            {
                // Create a fallback viewmodel
                _viewModel = new ServiceGridViewModel(null, CategoryChangedCommand)
                {
                    SelectedCategory = SelectedCategory,
                    Language = Language,
                    Theme = Theme
                };
                BindingContext = _viewModel;
                
                Console.WriteLine($"Error initializing ServiceGrid: {ex.Message}");
            }
        }

        private IServiceService GetServiceService()
        {
            // Try to get from DI first
            var service = App.Current?.Handler?.MauiContext?.Services.GetService<IServiceService>();
            if (service != null)
                return service;

            // Fallback to ServiceLocator
            try
            {
                return ServiceLocator.GetService<IServiceService>();
            }
            catch
            {
                // Create a mock service for design time
                return new MockServiceService();
            }
        }

        private static void OnSelectedCategoryChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ServiceGrid serviceGrid && newValue is string category)
            {
                serviceGrid._viewModel.SelectedCategory = category;
            }
        }

        private static void OnLanguageChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ServiceGrid serviceGrid && newValue is Language language)
            {
                serviceGrid._viewModel.Language = language;
            }
        }

        private static void OnThemeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ServiceGrid serviceGrid && newValue is Theme theme)
            {
                serviceGrid._viewModel.Theme = theme;
            }
        }

        protected override void OnParentChanged()
        {
            base.OnParentChanged();

            if (Parent == null && _viewModel != null)
            {
                _viewModel.Cleanup();
            }
        }
    }

    // Mock service for design time or fallback
    internal class MockServiceService : IServiceService
    {
        public Task<List<CategoryItem>> GetAllCategoriesAsync(Language language)
        {
            return Task.FromResult(new List<CategoryItem>
            {
                new CategoryItem { Id = 1, Type = "washing", Name = "WASH", Icon = "🧺", Color = "#3B82F6", IsActive = true },
                new CategoryItem { Id = 2, Type = "drying", Name = "DRY", Icon = "☀️", Color = "#F59E0B", IsActive = true },
                new CategoryItem { Id = 3, Type = "addon", Name = "ADD-ON", Icon = "➕", Color = "#10B981", IsActive = true },
                new CategoryItem { Id = 4, Type = "package", Name = "PACKAGE", Icon = "📦", Color = "#8B5CF6", IsActive = true }
            });
        }

        public Task<CategoryItem?> GetCategoryByTypeAsync(string type, Language language)
        {
            var categories = GetAllCategoriesAsync(language).Result;
            return Task.FromResult(categories.FirstOrDefault(c => c.Type == type));
        }

        public Task<List<ServiceItem>> GetServicesByCategoryAsync(string category, Language language)
        {
            var services = new List<ServiceItem>();
            
            if (category == "washing")
            {
                services.Add(new ServiceItem
                {
                    Id = "1",
                    Name = "Hot Water Wash",
                    Description = "Complete wash with hot water",
                    Price = 5000,
                    Icon = "🔥",
                    Color = "#FEE2E2",
                    Category = "washing",
                    IsAvailable = true
                });
                services.Add(new ServiceItem
                {
                    Id = "2",
                    Name = "Cold Water Wash",
                    Description = "Gentle wash with cold water",
                    Price = 3000,
                    Icon = "💧",
                    Color = "#DBEAFE",
                    Category = "washing",
                    IsAvailable = true
                });
            }
            
            return Task.FromResult(services);
        }

        public Task<ServiceItem?> GetServiceByIdAsync(string id, Language language)
        {
            return Task.FromResult<ServiceItem?>(null);
        }

        public Task InitializeDatabaseAsync()
        {
            return Task.CompletedTask;
        }
    }
}