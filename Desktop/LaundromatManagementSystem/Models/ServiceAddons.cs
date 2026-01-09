using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaundromatManagementSystem.Models
{
    public class ServiceAddons : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        
        [Required]
        public int ServiceId { get; set; }
        
        // Navigation property
        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; } = null!;
    }
}