using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaundromatManagementSystem.Entities
{
    [Table("PaymentRecords")]
    public class PaymentRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int TransactionId { get; set; }

        [Required]
        [MaxLength(20)]
        public string PaymentMethod { get; set; } = string.Empty; // cash, momo, card

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "pending"; // pending, completed, failed

        public double Amount { get; set; }

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; } // MoMo/Card reference

        [MaxLength(500)]
        public string? PaymentDetails { get; set; } // JSON details

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public DateTime? CompletionDate { get; set; }

        // Audit fields
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateDate { get; set; }

        // Navigation property
        [ForeignKey("TransactionId")]
        public virtual Transaction? Transaction { get; set; }
    }
}