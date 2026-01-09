using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LaundromatManagementSystem.Models
{
    public class ServiceCategory : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Type { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string? Description { get; set; }
        
        [Required]
        public int DisplayOrder { get; set; } = 1;
        
        // Navigation property
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    }
}