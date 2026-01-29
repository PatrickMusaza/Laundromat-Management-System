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
        private string _selectedCategory;

        [ObservableProperty]
        private Language _language;

        [ObservableProperty]
        private Theme _theme;

        [ObservableProperty]
        private ObservableCollection<CategoryViewModel> _categories = new();

        [ObservableProperty]
        private ObservableCollection<ServiceViewModel> _services = new();

        [ObservableProperty]
        private bool _isLoading;

        public ICommand CategoryChangedCommand { get; }

        public ServiceGridViewModel(IServiceService serviceService,
                                   ICommand categoryChangedCommand)
        {
            _serviceService = serviceService;
            CategoryChangedCommand = categoryChangedCommand;

            // Initialize from state service
            _language = _stateService.CurrentLanguage;
            _theme = _stateService.CurrentTheme;

            // Subscribe to state changes
            _stateService.PropertyChanged += OnStateChanged;

            // Load categories and services
            LoadCategories();
            LoadServices();
        }

        // Override setters to update state service
        partial void OnLanguageChanged(Language value)
        {
            if (_stateService.CurrentLanguage != value)
            {
                _stateService.CurrentLanguage = value;
            }
            LoadCategories();
            LoadServices();
        }

        partial void OnThemeChanged(Theme value)
        {
            if (_stateService.CurrentTheme != value)
            {
                _stateService.CurrentTheme = value;
            }
            UpdateThemeProperties();
        }

        partial void OnSelectedCategoryChanged(string value)
        {
            UpdateSelectedCategory();
            CategoryChangedCommand?.Execute(value);
            LoadServices();
        }

        private void UpdateThemeProperties()
        {
            // Update all category viewmodels
            foreach (var category in Categories)
            {
                category.Theme = Theme;
            }

            // Update all service viewmodels
            foreach (var service in Services)
            {
                service.Theme = Theme;
            }
        }

        private void UpdateSelectedCategory()
        {
            // Update selection state for all categories
            foreach (var category in Categories)
            {
                category.IsSelected = category.Type == SelectedCategory;
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

        [RelayCommand]
        private void SelectCategory(string categoryType)
        {
            SelectedCategory = categoryType;
        }

        private async void LoadCategories()
        {
            try
            {
                var categoryItems = await _serviceService.GetAllCategoriesAsync(Language);
                Categories.Clear();

                foreach (var item in categoryItems)
                {
                    var categoryVm = new CategoryViewModel(
                        item,
                        new RelayCommand<string>(SelectCategory),
                        Theme,
                        item.Type == SelectedCategory
                    );
                    Categories.Add(categoryVm);
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load categories: {ex.Message}", "OK");
            }
        }

        private async void LoadServices()
        {
            IsLoading = true;

            try
            {
                var serviceItems = await _serviceService.GetServicesByCategoryAsync(SelectedCategory, Language);
                Services.Clear();

                foreach (var item in serviceItems)
                {
                    if (item.IsAvailable)
                    {
                        Services.Add(new ServiceViewModel(item, Theme, Language));
                    }
                }

                foreach (var service in Services)
                {
                    service.Theme = Theme;
                    service.Language = Language;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to load services: {ex.Message}", "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // Clean up
        public void Cleanup()
        {
            _stateService.PropertyChanged -= OnStateChanged;

            // Clean up all service viewmodels
            foreach (var service in Services)
            {
                service.Cleanup();
            }
        }
    }
}