using LaundromatManagementSystem.Models;
using LaundromatManagementSystem.Services;
using System.Collections.ObjectModel;

namespace LaundromatManagementSystem.Views
{
    public partial class ServiceGrid : ContentView
    {
        public ObservableCollection<ServiceItem> Services { get; } = new();
        
        public ServiceGrid()
        {
            InitializeComponent();
            
            // Load initial services
            LoadWashingServices();
            ServicesCollection.ItemsSource = Services;
        }
        
        private void LoadWashingServices()
        {
            Services.Clear();
            
            // Washing services
            Services.Add(new ServiceItem
            {
                Id = "hot-water",
                Name = "Hot Water Wash",
                Description = "",
                Price = 5000,
                Icon = "🔥",
                Category = "washing"
            });
            
            Services.Add(new ServiceItem
            {
                Id = "cold-water",
                Name = "Cold Water Wash",
                Description = "",
                Price = 6000,
                Icon = "💧",
                Category = "washing"
            });
            
            Services.Add(new ServiceItem
            {
                Id = "express-wash",
                Name = "Express Wash",
                Description = "",
                Price = 8000,
                Icon = "⚡",
                Category = "washing"
            });
            
            UpdateButtonColors("washing");
        }
        
        private void LoadDryingServices()
        {
            Services.Clear();
            
            // Drying services
            Services.Add(new ServiceItem
            {
                Id = "regular-dry",
                Name = "Regular Dry",
                Description = "",
                Price = 3000,
                Icon = "🌀",
                Category = "drying"
            });
            
            Services.Add(new ServiceItem
            {
                Id = "heavy-dry",
                Name = "Heavy Duty Dry",
                Description = "",
                Price = 5000,
                Icon = "🌀",
                Category = "drying"
            });
            
            UpdateButtonColors("drying");
        }
        
        private void LoadAddonServices()
        {
            Services.Clear();
            
            // Addon services
            Services.Add(new ServiceItem
            {
                Id = "ironing",
                Name = "Ironing",
                Description = "",
                Price = 1000,
                Icon = "👔",
                Category = "addon"
            });
            
            Services.Add(new ServiceItem
            {
                Id = "bleach",
                Name = "Bleach Treatment (FREE)",
                Description = "",
                Price = 0,
                Icon = "⭐",
                Category = "addon"
            });
            
            UpdateButtonColors("addon");
        }
        
        private void LoadPackageServices()
        {
            Services.Clear();
            
            // Package services
            Services.Add(new ServiceItem
            {
                Id = "complete-package",
                Name = "Complete Package",
                Description = "Wash + Dry + Iron + Bleach",
                Price = 12000,
                Icon = "📦",
                Category = "package"
            });
            
            UpdateButtonColors("package");
        }
        
        private void UpdateButtonColors(string selectedCategory)
        {
            // Reset all buttons
            WashButton.BackgroundColor = Color.FromArgb("#FFFFFF");
            WashButton.Stroke = Color.FromArgb("#E5E7EB");
            ((Label)WashButton.Content).TextColor = Color.FromArgb("#6B7280");
            
            DryButton.BackgroundColor = Color.FromArgb("#FFFFFF");
            DryButton.Stroke = Color.FromArgb("#E5E7EB");
            ((Label)DryButton.Content).TextColor = Color.FromArgb("#6B7280");
            
            AddonButton.BackgroundColor = Color.FromArgb("#FFFFFF");
            AddonButton.Stroke = Color.FromArgb("#E5E7EB");
            ((Label)AddonButton.Content).TextColor = Color.FromArgb("#6B7280");
            
            PackageButton.BackgroundColor = Color.FromArgb("#FFFFFF");
            PackageButton.Stroke = Color.FromArgb("#E5E7EB");
            ((Label)PackageButton.Content).TextColor = Color.FromArgb("#6B7280");
            
            // Highlight selected button
            switch (selectedCategory)
            {
                case "washing":
                    WashButton.BackgroundColor = Color.FromArgb("#1E3A8A");
                    WashButton.Stroke = Colors.Transparent;
                    ((Label)WashButton.Content).TextColor = Colors.White;
                    break;
                    
                case "drying":
                    DryButton.BackgroundColor = Color.FromArgb("#1E3A8A");
                    DryButton.Stroke = Colors.Transparent;
                    ((Label)DryButton.Content).TextColor = Colors.White;
                    break;
                    
                case "addon":
                    AddonButton.BackgroundColor = Color.FromArgb("#1E3A8A");
                    AddonButton.Stroke = Colors.Transparent;
                    ((Label)AddonButton.Content).TextColor = Colors.White;
                    break;
                    
                case "package":
                    PackageButton.BackgroundColor = Color.FromArgb("#1E3A8A");
                    PackageButton.Stroke = Colors.Transparent;
                    ((Label)PackageButton.Content).TextColor = Colors.White;
                    break;
            }
        }
        
        private void OnWashTapped(object sender, EventArgs e)
        {
            LoadWashingServices();
        }
        
        private void OnDryTapped(object sender, EventArgs e)
        {
            LoadDryingServices();
        }
        
        private void OnAddonTapped(object sender, EventArgs e)
        {
            LoadAddonServices();
        }
        
        private void OnPackageTapped(object sender, EventArgs e)
        {
            LoadPackageServices();
        }
        
        private void OnServiceTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is ServiceItem service)
            {
                // Create a cart item
                var cartItem = new CartItem
                {
                    Id = $"{service.Id}-{Guid.NewGuid():N}",
                    Name = service.Name,
                    Price = service.Price,
                    Quantity = 1
                };
                
                // Find the parent Dashboard to call OnServiceTapped
                var parent = this.Parent;
                while (parent != null && parent is not Dashboard)
                {
                    parent = parent.Parent;
                }
                
                if (parent is Dashboard dashboard)
                {
                    dashboard.OnServiceTapped(cartItem);
                }
            }
        }
    }
    
    public class ServiceItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool HasDescription => !string.IsNullOrEmpty(Description);
    }
}