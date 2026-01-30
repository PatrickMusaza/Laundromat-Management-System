namespace LaundromatManagementSystem.Models
{
    public class PaymentResult
    {
        public PaymentMethod PaymentMethod { get; set; }
        public string Customer { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Amount { get; set; }
        public decimal Change { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerTin { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public List<CartItem> Items { get; set; } = new();
        public double CashReceived { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    }

    public enum PaymentMethod
    {
        Cash,
        MoMo,
        Card
    }
}