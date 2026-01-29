using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaundromatManagementSystem.Entities
{
    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string TransactionId { get; set; } = string.Empty; // T-123456

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "pending"; // pending, completed, cancelled, refunded

        [Required]
        [MaxLength(20)]
        public string PaymentMethod { get; set; } = string.Empty; // cash, momo, card

        public double Subtotal { get; set; }
        public double TaxAmount { get; set; }
        public double TotalAmount { get; set; }
        public double? CashReceived { get; set; }
        public double? ChangeAmount { get; set; }

        [MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string CustomerTin { get; set; } = string.Empty;

        [MaxLength(50)]
        public string CustomerPhone { get; set; } = string.Empty;

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public DateTime? PaymentDate { get; set; }
        public DateTime? CompletionDate { get; set; }

        // Audit fields
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateDate { get; set; }
        public string? UpdatedBy { get; set; }

        // Navigation properties
        public virtual ICollection<TransactionItem> Items { get; set; } = new List<TransactionItem>();
        public virtual ICollection<PaymentRecord> Payments { get; set; } = new List<PaymentRecord>();
    }
}