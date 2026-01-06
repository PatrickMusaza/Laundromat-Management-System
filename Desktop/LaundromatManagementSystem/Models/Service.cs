namespace LaundromatManagementSystem.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ServiceType Type { get; set; }
        public decimal Price { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
        
        // Addons property
        public List<ServiceAddon> Addons { get; set; } = new();
        public bool HasAddons => Addons.Count > 0;
    }
    
    public class ServiceAddon
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
    
    public enum ServiceType
    {
        Wash,
        Dry,
        AddOn,
        Package
    }
}