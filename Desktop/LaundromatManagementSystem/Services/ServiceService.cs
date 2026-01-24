using LaundromatManagementSystem.ViewModels;

namespace LaundromatManagementSystem.Services
{
    public class ServiceService : IServiceService
    {
        public Task<List<ServiceItem>> GetServicesByCategoryAsync(string category, Language language)
        {
            var services = GetAllServices(language);
            var filtered = services.Where(s => s.Category == category).ToList();
            return Task.FromResult(filtered);
        }

        public Task<List<ServiceItem>> GetAllServicesAsync(Language language)
        {
            return Task.FromResult(GetAllServices(language));
        }

        private List<ServiceItem> GetAllServices(Language language)
        {
            var services = new List<ServiceItem>();

            // Washing services (exact from TypeScript)
            services.AddRange(new[]
            {
                new ServiceItem
                {
                    Id = "hot-water",
                    Name = GetServiceName("Hot Water", language),
                    Description = "",
                    Price = 5000,
                    Icon = "🔥",
                    Category = "washing"
                },
                new ServiceItem
                {
                    Id = "cold-water",
                    Name = GetServiceName("Cold Water", language),
                    Description = "",
                    Price = 6000,
                    Icon = "💧",
                    Category = "washing"
                },
                new ServiceItem
                {
                    Id = "express-wash",
                    Name = GetServiceName("Express Wash", language),
                    Description = "",
                    Price = 8000,
                    Icon = "⚡",
                    Category = "washing"
                }
            });

            // Drying services
            services.AddRange(new[]
            {
                new ServiceItem
                {
                    Id = "regular-dry",
                    Name = GetServiceName("Regular Dry", language),
                    Description = "",
                    Price = 3000,
                    Icon = "🌀",
                    Category = "drying"
                },
                new ServiceItem
                {
                    Id = "heavy-dry",
                    Name = GetServiceName("Heavy Duty Dry", language),
                    Description = "",
                    Price = 5000,
                    Icon = "🌀",
                    Category = "drying"
                }
            });

            // Addon services
            services.AddRange(new[]
            {
                new ServiceItem
                {
                    Id = "ironing",
                    Name = GetServiceName("Ironing", language),
                    Description = "",
                    Price = 1000,
                    Icon = "👔",
                    Category = "addon"
                },
                new ServiceItem
                {
                    Id = "bleach",
                    Name = GetServiceName("Bleach Treatment (FREE)", language),
                    Description = "",
                    Price = 0,
                    Icon = "⭐",
                    Category = "addon"
                }
            });

            // Package services
            services.Add(new ServiceItem
            {
                Id = "complete-package",
                Name = GetServiceName("Complete Package", language),
                Description = GetPackageDescription(language),
                Price = 12000,
                Icon = "📦",
                Category = "package"
            });

            return services;
        }

        private string GetServiceName(string englishName, Language language)
        {
            var translations = new Dictionary<string, Dictionary<Language, string>>
            {
                ["Hot Water"] = new()
                {
                    [Language.EN] = "Hot Water",
                    [Language.RW] = "Amazi Ashyushye",
                    [Language.FR] = "Eau Chaude"
                },
                ["Cold Water"] = new()
                {
                    [Language.EN] = "Cold Water",
                    [Language.RW] = "Amazi Akonje",
                    [Language.FR] = "Eau Froide"
                },
                ["Express Wash"] = new()
                {
                    [Language.EN] = "Express Wash",
                    [Language.RW] = "Karaba Vuba",
                    [Language.FR] = "Lavage Express"
                },
                ["Regular Dry"] = new()
                {
                    [Language.EN] = "Regular Dry",
                    [Language.RW] = "Umisha Bisanzwe",
                    [Language.FR] = "Séchage Normal"
                },
                ["Heavy Duty Dry"] = new()
                {
                    [Language.EN] = "Heavy Duty Dry",
                    [Language.RW] = "Umisha Biremereye",
                    [Language.FR] = "Séchage Intense"
                },
                ["Ironing"] = new()
                {
                    [Language.EN] = "Ironing",
                    [Language.RW] = "Gusukura",
                    [Language.FR] = "Repassage"
                },
                ["Bleach Treatment (FREE)"] = new()
                {
                    [Language.EN] = "Bleach Treatment (FREE)",
                    [Language.RW] = "Kurera (Ubuntu)",
                    [Language.FR] = "Traitement Javel (GRATUIT)"
                },
                ["Complete Package"] = new()
                {
                    [Language.EN] = "Complete Package",
                    [Language.RW] = "Paki Yuzuye",
                    [Language.FR] = "Forfait Complet"
                }
            };

            return translations[englishName][language];
        }

        private string GetPackageDescription(Language language)
        {
            return language switch
            {
                Language.EN => "Wash + Dry + Iron + Bleach",
                Language.RW => "Karaba + Umisha + Gusukura",
                Language.FR => "Laver + Sécher + Repasser + Javel",
                _ => "Wash + Dry + Iron + Bleach"
            };
        }
    }
}