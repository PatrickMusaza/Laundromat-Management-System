using LaundromatManagementSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace LaundromatManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<ServiceCategory> ServiceCategories { get; set; }
        public DbSet<Service> Services { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            // Get database path
            string databasePath = GetDatabasePath();

            // Enable SQLite encryption with a password (optional but recommended)
            var connectionString = $"Data Source={databasePath};Password=SecurePassword123!";

            optionsBuilder.UseSqlite(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Service entity
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasIndex(e => e.Type);
                entity.HasIndex(e => e.IsAvailable);
                entity.HasIndex(e => e.ServiceCategoryId);
            });

            // Configure ServiceCategory entity
            modelBuilder.Entity<ServiceCategory>(entity =>
            {
                entity.HasIndex(e => e.Type).IsUnique();
            });

            // Seed initial data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Service Categories
            modelBuilder.Entity<ServiceCategory>().HasData(
                new ServiceCategory { Id = 1, Type = "washing" },
                new ServiceCategory { Id = 2, Type = "drying" },
                new ServiceCategory { Id = 3, Type = "addon" },
                new ServiceCategory { Id = 4, Type = "package" }
            );

            // Seed Services
            modelBuilder.Entity<Service>().HasData(
                new Service
                {
                    Id = 1,
                    Name = "Hot Water Wash",
                    NameEn = "Hot Water Wash",
                    NameRw = "Karaba y'amazi ashyushye",
                    NameFr = "Lavage à l'eau chaude",
                    Type = "washing",
                    Price = 5000.00,
                    Icon = "🔥",
                    Color = "#FEE2E2",
                    IsAvailable = true,
                    DescriptionEn = "Complete wash with hot water",
                    DescriptionRw = "Karaba yuzuye hamwe n'amazi ashyushye",
                    DescriptionFr = "Lavage complet avec de l'eau chaude",
                    ServiceCategoryId = 1
                },
                new Service
                {
                    Id = 2,
                    Name = "Cold Water Wash",
                    NameEn = "Cold Water Wash",
                    NameRw = "Karaba y'amazi konje",
                    NameFr = "Lavage à l'eau froide",
                    Type = "washing",
                    Price = 3000.00,
                    Icon = "💧",
                    Color = "#DBEAFE",
                    IsAvailable = true,
                    DescriptionEn = "Gentle wash with cold water",
                    DescriptionRw = "Karaba buhoro buhoro hamwe n'amazi konje",
                    DescriptionFr = "Lavage doux à l'eau froide",
                    ServiceCategoryId = 1
                },
                new Service
                {
                    Id = 3,
                    Name = "Express Dry",
                    NameEn = "Express Dry",
                    NameRw = "Umisha byihuse",
                    NameFr = "Séchage express",
                    Type = "drying",
                    Price = 2500.00,
                    Icon = "⚡",
                    Color = "#FEF3C7",
                    IsAvailable = true,
                    DescriptionEn = "Quick drying service",
                    DescriptionRw = "Serivisi yo gukangura umisha",
                    DescriptionFr = "Service de séchage rapide",
                    ServiceCategoryId = 2
                },
                new Service
                {
                    Id = 4,
                    Name = "Premium Detergent",
                    NameEn = "Premium Detergent",
                    NameRw = "Detero ntarengwa",
                    NameFr = "Détergent premium",
                    Type = "addon",
                    Price = 1000.00,
                    Icon = "🌟",
                    Color = "#D1FAE5",
                    IsAvailable = true,
                    DescriptionEn = "High-quality detergent",
                    DescriptionRw = "Detero y'ikirenga",
                    DescriptionFr = "Détergent de haute qualité",
                    ServiceCategoryId = 3
                }
            );
        }

        private string GetDatabasePath()
        {
            string databasePath;

            // For MAUI apps
            if (DeviceInfo.Platform == DevicePlatform.Unknown)
            {
                // For development/debugging
                databasePath = Path.Combine(FileSystem.AppDataDirectory, "laundromat.db3");
            }
            else
            {
                databasePath = Path.Combine(FileSystem.AppDataDirectory, "laundromat.db3");
            }

            // Ensure directory exists
            var directory = Path.GetDirectoryName(databasePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return databasePath;
        }
    }
}