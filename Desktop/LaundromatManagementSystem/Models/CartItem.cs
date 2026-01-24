using System.Collections.ObjectModel;

namespace LaundromatManagementSystem.Models
{
    public class CartItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public ObservableCollection<ServiceAddon> Addons { get; set; } = new();
        public int Quantity { get; set; } = 1;
        public int ServiceId { get; set; } = 0;
        
        public decimal TotalPrice => (Price + Addons.Sum(a => a.Price)) * Quantity;
    }
    
    public class ServiceAddon
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}