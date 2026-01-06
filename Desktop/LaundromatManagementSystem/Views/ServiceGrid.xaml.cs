using System.Windows.Input;
using System.Collections.ObjectModel;
using LaundromatManagementSystem.Models;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LaundromatManagementSystem.Views
{
    public partial class ServiceGrid : ContentView
    {
        public static readonly BindableProperty SelectedCategoryProperty =
            BindableProperty.Create(nameof(SelectedCategory), typeof(string), typeof(ServiceGrid), "washing");
        
        public static readonly BindableProperty LanguageProperty =
            BindableProperty.Create(nameof(Language), typeof(string), typeof(ServiceGrid), "EN");
        
        public static readonly BindableProperty ThemeProperty =
            BindableProperty.Create(nameof(Theme), typeof(string), typeof(ServiceGrid), "light");
        
        public static readonly BindableProperty AddToCartCommandProperty =
            BindableProperty.Create(nameof(AddToCartCommand), typeof(ICommand), typeof(ServiceGrid));
        
        public static readonly BindableProperty ServicesProperty =
            BindableProperty.Create(nameof(Services), typeof(ObservableCollection<Service>), 
                typeof(ServiceGrid), new ObservableCollection<Service>());
        
        public string SelectedCategory
        {
            get => (string)GetValue(SelectedCategoryProperty);
            set => SetValue(SelectedCategoryProperty, value);
        }
        
        public string Language
        {
            get => (string)GetValue(LanguageProperty);
            set => SetValue(LanguageProperty, value);
        }
        
        public string Theme
        {
            get => (string)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }
        
        public ICommand AddToCartCommand
        {
            get => (ICommand)GetValue(AddToCartCommandProperty);
            set => SetValue(AddToCartCommandProperty, value);
        }
        
        public ObservableCollection<Service> Services
        {
            get => (ObservableCollection<Service>)GetValue(ServicesProperty);
            set => SetValue(ServicesProperty, value);
        }
        
        public ServiceGrid()
        {
            InitializeComponent();
            BindingContext = this;
            LoadSampleServices();
        }
        
        private void LoadSampleServices()
        {
            Services.Clear();
            
            // Sample services
            Services.Add(new Service
            {
                Id = 1,
                Name = "Hot Water Wash",
                Description = "Hot water wash with detergent",
                Price = 5000,
                Type = ServiceType.Wash
            });
            
            Services.Add(new Service
            {
                Id = 2,
                Name = "Cold Water Wash",
                Description = "Cold water wash with detergent",
                Price = 6000,
                Type = ServiceType.Wash
            });
            
            Services.Add(new Service
            {
                Id = 3,
                Name = "Express Wash",
                Description = "Fast wash service (30 minutes)",
                Price = 8000,
                Type = ServiceType.Wash
            });
            
            Services.Add(new Service
            {
                Id = 4,
                Name = "Dry Cleaning",
                Description = "Professional dry cleaning",
                Price = 10000,
                Type = ServiceType.Dry
            });
            
            Services.Add(new Service
            {
                Id = 5,
                Name = "Ironing",
                Description = "Clothes ironing service",
                Price = 3000,
                Type = ServiceType.AddOn
            });
            
            Services.Add(new Service
            {
                Id = 6,
                Name = "Premium Package",
                Description = "Wash + Dry + Ironing package",
                Price = 15000,
                Type = ServiceType.Package
            });
        }
    }
}