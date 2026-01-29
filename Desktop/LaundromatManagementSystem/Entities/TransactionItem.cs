using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaundromatManagementSystem.Entities
{
    [Table("TransactionItems")]
    public class TransactionItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int TransactionId { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [MaxLength(100)]
        public string ServiceName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ServiceDescription { get; set; }

        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }

        [MaxLength(50)]
        public string ServiceType { get; set; } = string.Empty; // washing, drying, etc.

        [MaxLength(10)]
        public string ServiceIcon { get; set; } = string.Empty;

        // Audit fields
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey("TransactionId")]
        public virtual Transaction? Transaction { get; set; }

        [ForeignKey("ServiceId")]
        public virtual Service? Service { get; set; }
    }
}