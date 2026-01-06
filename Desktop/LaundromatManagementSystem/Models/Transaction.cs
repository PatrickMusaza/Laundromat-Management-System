using System.Collections.ObjectModel;

namespace LaundromatManagementSystem.Models
{
    public class Transaction
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string Status { get; set; } = "completed";
        public ObservableCollection<CartItem> Items { get; set; } = new();
        public string Customer { get; set; } = string.Empty;
    }
}