namespace LaundromatManagementSystem.Models
{
    public class Transaction
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<CartItem> Items { get; set; } = new();
        public string Customer { get; set; } = string.Empty;
    }
}