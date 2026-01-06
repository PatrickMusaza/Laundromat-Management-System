namespace LaundromatManagementSystem.Models
{
    public class CartItem
    {
        public Service Service { get; set; } = new();
        public int Quantity { get; set; } = 1;
        public decimal TotalPrice => Service.Price * Quantity;
        
        // Use a simpler ID approach
        public string Id => $"{Service.Id}-{Quantity}";
    }
}