namespace LaundromatManagementSystem.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ServiceType Type { get; set; }
        public decimal Price { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
    
    public enum ServiceType
    {
        Wash,
        Dry,
        AddOn,
        Package
    }
}