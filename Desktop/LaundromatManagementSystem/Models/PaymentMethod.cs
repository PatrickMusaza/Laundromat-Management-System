using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LaundromatManagementSystem.Models
{
    public class PaymentMethod : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(20)]
        public string Color { get; set; } = "#3B82F6";
        
        [StringLength(100)]
        public string? Icon { get; set; }
        
        [Required]
        public bool IsActive { get; set; } = true;
        
        // Encrypted field (will be encrypted in DbContext)
        [Required]
        public string SerialNumber { get; set; } = string.Empty;
        
        // Navigation property
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}