using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaundromatManagementSystem.Models
{
    public class Service : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Icon { get; set; } = "🔄";
        
        [StringLength(20)]
        public string Color { get; set; } = "#3B82F6";
        
        [Required]
        public bool IsAvailable { get; set; } = true;
        
        [Required]
        public int CategoryId { get; set; }
        
        // Navigation properties
        [ForeignKey("CategoryId")]
        public virtual ServiceCategory Category { get; set; } = null!;
        
        public virtual ICollection<ServiceAddon> Addons { get; set; } = new List<ServiceAddon>();
    }
}