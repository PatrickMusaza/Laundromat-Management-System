namespace LaundromatManagementSystem.Models
{
    public class CartItem
    {
        public Service Service { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Service.Price * Quantity;
    }
}