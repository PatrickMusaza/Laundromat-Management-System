using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaundromatManagementSystem.Entities
{
    [Table("Services")]
    public class Service
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty; // washing, drying, addon, package

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [MaxLength(10)]
        public string Icon { get; set; } = string.Empty; // emoji or icon code

        [MaxLength(20)]
        public string Color { get; set; } = string.Empty; // hex color

        public bool IsAvailable { get; set; } = true;

        // Translations
        [MaxLength(200)]
        public string NameEn { get; set; } = string.Empty;

        [MaxLength(200)]
        public string NameRw { get; set; } = string.Empty;

        [MaxLength(200)]
        public string NameFr { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? DescriptionEn { get; set; }

        [MaxLength(500)]
        public string? DescriptionRw { get; set; }

        [MaxLength(500)]
        public string? DescriptionFr { get; set; }

        // Audit fields
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateDate { get; set; }
        public string? UpdatedBy { get; set; }

        // Foreign key
        public int ServiceCategoryId { get; set; }

        [ForeignKey("ServiceCategoryId")]
        public virtual ServiceCategory? Category { get; set; }
    }
}