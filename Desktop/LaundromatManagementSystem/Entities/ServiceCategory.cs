using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LaundromatManagementSystem.Entities
{
    [Table("ServiceCategories")]
    public class ServiceCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty; // washing, drying, addon, package

        [MaxLength(50)]
        public string? Icon { get; set; } // Icon for category

        [MaxLength(20)]
        public string Color { get; set; } = string.Empty; // Color for category

        // Translations
        [MaxLength(100)]
        public string NameEn { get; set; } = string.Empty;

        [MaxLength(100)]
        public string NameRw { get; set; } = string.Empty;

        [MaxLength(100)]
        public string NameFr { get; set; } = string.Empty;

        public int SortOrder { get; set; } = 0; // For ordering categories

        public bool IsActive { get; set; } = true;

        // Audit fields
        public DateTime CreateDate { get; set; } = DateTime.UtcNow;
        public DateTime? UpdateDate { get; set; }
        public string? UpdatedBy { get; set; }

        // Navigation property
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();
    }
}