namespace LaundromatManagementSystem.Models
{
    public class CategoryItem
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        // Helper method to create from entity
        public static CategoryItem FromEntity(Entities.ServiceCategory entity, Language language)
        {
            return new CategoryItem
            {
                Id = entity.Id,
                Type = entity.Type,
                Name = GetTranslatedName(entity, language),
                Icon = entity.Icon,
                Color = entity.Color,
                IsActive = entity.IsActive
            };
        }

        private static string GetTranslatedName(Entities.ServiceCategory entity, Language language)
        {
            return language switch
            {
                Language.EN => entity.NameEn,
                Language.RW => entity.NameRw,
                Language.FR => entity.NameFr,
                _ => entity.Type.ToUpper()
            };
        }
    }
}