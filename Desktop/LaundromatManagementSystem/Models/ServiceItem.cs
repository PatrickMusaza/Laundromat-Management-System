namespace LaundromatManagementSystem.Models
{
    public class ServiceItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Icon { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;

        // Helper method to create from entity
        public static ServiceItem FromEntity(Entities.Service entity, Language language)
        {
            return new ServiceItem
            {
                Id = entity.Id.ToString(),
                Name = GetTranslatedName(entity, language),
                Description = GetTranslatedDescription(entity, language) ?? string.Empty,
                Price = entity.Price,
                Icon = entity.Icon,
                Category = entity.Type,
                Color = entity.Color
            };
        }

        private static string GetTranslatedName(Entities.Service entity, Language language)
        {
            return language switch
            {
                Language.EN => entity.NameEn,
                Language.RW => entity.NameRw,
                Language.FR => entity.NameFr,
                _ => entity.Name
            };
        }

        private static string? GetTranslatedDescription(Entities.Service entity, Language language)
        {
            return language switch
            {
                Language.EN => entity.DescriptionEn,
                Language.RW => entity.DescriptionRw,
                Language.FR => entity.DescriptionFr,
                _ => entity.DescriptionEn
            };
        }
    }

    public enum Language
    {
        EN,
        RW,
        FR
    }

    public enum Theme
    {
        Light,
        Dark,
        Gray
    }
}