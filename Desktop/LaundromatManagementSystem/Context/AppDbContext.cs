// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.ChangeTracking;
// using Microsoft.Data.Sqlite;
// using System.Text;
// using System.Security.Cryptography;
// using LaundromatManagementSystem.Models;

// namespace LaundromatManagementSystem.Data
// {
//     public class AppDbContext : DbContext
//     {
//         private static readonly byte[] EncryptionKey = GenerateEncryptionKey();
//         private static readonly byte[] EncryptionIV = GenerateEncryptionIV();
        
//         public DbSet<ServiceCategory> ServiceCategories { get; set; }
//         public DbSet<Service> Services { get; set; }
//         public DbSet<ServiceAddon> ServiceAddons { get; set; }
//         public DbSet<PaymentMethod> PaymentMethods { get; set; }
//         public DbSet<CartItem> CartItems { get; set; }
//         public DbSet<CartItemAddon> CartItemAddons { get; set; }
//         public DbSet<Transaction> Transactions { get; set; }
//         public DbSet<SalesRRA> SalesRRAs { get; set; }
        
//         public AppDbContext()
//         {
//             // Ensure database is created
//             Database.EnsureCreated();
//         }
        
//         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//         {
//             string dbPath = Path.Combine(FileSystem.AppDataDirectory, "laundromat.db");
//             var connectionString = new SqliteConnectionStringBuilder
//             {
//                 DataSource = dbPath,
//                 Password = "YourSecurePassword123!", // Add password for encryption
//                 Mode = SqliteOpenMode.ReadWriteCreate,
//                 Cache = SqliteCacheMode.Shared
//             }.ToString();
            
//             optionsBuilder.UseSqlite(connectionString);
            
//             // Enable sensitive data logging for debugging (remove in production)
//             #if DEBUG
//             optionsBuilder.EnableSensitiveDataLogging();
//             #endif
//         }
        
//         protected override void OnModelCreating(ModelBuilder modelBuilder)
//         {
//             base.OnModelCreating(modelBuilder);
            
//             // Configure relationships
//             modelBuilder.Entity<Service>()
//                 .HasOne(s => s.Category)
//                 .WithMany(c => c.Services)
//                 .HasForeignKey(s => s.CategoryId)
//                 .OnDelete(DeleteBehavior.Restrict);
            
//             modelBuilder.Entity<ServiceAddon>()
//                 .HasOne(sa => sa.Service)
//                 .WithMany(s => s.Addons)
//                 .HasForeignKey(sa => sa.ServiceId)
//                 .OnDelete(DeleteBehavior.Cascade);
            
//             modelBuilder.Entity<CartItem>()
//                 .HasOne(ci => ci.Service)
//                 .WithMany()
//                 .HasForeignKey(ci => ci.ServiceId)
//                 .OnDelete(DeleteBehavior.Restrict);
            
//             modelBuilder.Entity<CartItemAddon>()
//                 .HasOne(cia => cia.CartItem)
//                 .WithMany(ci => ci.Addons)
//                 .HasForeignKey(cia => cia.CartItemId)
//                 .OnDelete(DeleteBehavior.Cascade);
            
//             modelBuilder.Entity<CartItemAddon>()
//                 .HasOne(cia => cia.Addon)
//                 .WithMany()
//                 .HasForeignKey(cia => cia.AddonId)
//                 .OnDelete(DeleteBehavior.Restrict);
            
//             modelBuilder.Entity<Transaction>()
//                 .HasOne(t => t.PaymentMethod)
//                 .WithMany(pm => pm.Transactions)
//                 .HasForeignKey(t => t.PaymentMethodId)
//                 .OnDelete(DeleteBehavior.Restrict);
            
//             modelBuilder.Entity<Transaction>()
//                 .HasOne(t => t.CartItem)
//                 .WithMany(ci => ci.Transactions)
//                 .HasForeignKey(t => t.CartItemId)
//                 .OnDelete(DeleteBehavior.SetNull);
            
//             modelBuilder.Entity<SalesRRA>()
//                 .HasOne(sr => sr.Transaction)
//                 .WithOne(t => t.SalesRRA)
//                 .HasForeignKey<SalesRRA>(sr => sr.TransactionId)
//                 .OnDelete(DeleteBehavior.Cascade);
            
//             // Configure value conversions for encryption
//             modelBuilder.Entity<PaymentMethod>()
//                 .Property(pm => pm.SerialNumber)
//                 .HasConversion(
//                     v => EncryptString(v),
//                     v => DecryptString(v));
            
//             modelBuilder.Entity<SalesRRA>()
//                 .Property(sr => sr.EncryptedTaxData)
//                 .HasConversion(
//                     v => EncryptString(v ?? string.Empty),
//                     v => DecryptString(v));
            
//             modelBuilder.Entity<Transaction>()
//                 .Property(t => t.PaymentDetails)
//                 .HasConversion(
//                     v => EncryptString(v ?? string.Empty),
//                     v => DecryptString(v));
            
//             // Seed initial data
//             SeedData(modelBuilder);
//         }
        
//         public override int SaveChanges()
//         {
//             UpdateTimestamps();
//             return base.SaveChanges();
//         }
        
//         public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
//         {
//             UpdateTimestamps();
//             return base.SaveChangesAsync(cancellationToken);
//         }
        
//         private void UpdateTimestamps()
//         {
//             var entries = ChangeTracker.Entries()
//                 .Where(e => e.Entity is BaseEntity && (
//                     e.State == EntityState.Added ||
//                     e.State == EntityState.Modified));
            
//             foreach (var entry in entries)
//             {
//                 var entity = (BaseEntity)entry.Entity;
                
//                 if (entry.State == EntityState.Added)
//                 {
//                     entity.CreatedAt = DateTime.UtcNow;
//                 }
                
//                 entity.UpdatedAt = DateTime.UtcNow;
                
//                 // In a real app, you'd set UpdatedBy from current user
//                 // entity.UpdatedBy = GetCurrentUsername();
//             }
//         }
        
//         private static void SeedData(ModelBuilder modelBuilder)
//         {
//             // Seed Service Categories
//             modelBuilder.Entity<ServiceCategory>().HasData(
//                 new ServiceCategory { Id = 1, Type = "washing", Description = "Washing Services", DisplayOrder = 1 },
//                 new ServiceCategory { Id = 2, Type = "drying", Description = "Drying Services", DisplayOrder = 2 },
//                 new ServiceCategory { Id = 3, Type = "addon", Description = "Additional Services", DisplayOrder = 3 },
//                 new ServiceCategory { Id = 4, Type = "package", Description = "Package Deals", DisplayOrder = 4 }
//             );
            
//             // Seed Payment Methods
//             modelBuilder.Entity<PaymentMethod>().HasData(
//                 new PaymentMethod 
//                 { 
//                     Id = 1, 
//                     Name = "Cash", 
//                     Color = "#10B981", 
//                     Icon = "💵",
//                     SerialNumber = "CASH-001",
//                     IsActive = true
//                 },
//                 new PaymentMethod 
//                 { 
//                     Id = 2, 
//                     Name = "MoMo", 
//                     Color = "#8B5CF6", 
//                     Icon = "📱",
//                     SerialNumber = "MOMO-002",
//                     IsActive = true
//                 },
//                 new PaymentMethod 
//                 { 
//                     Id = 3, 
//                     Name = "Card", 
//                     Color = "#3B82F6", 
//                     Icon = "💳",
//                     SerialNumber = "CARD-003",
//                     IsActive = true
//                 }
//             );
//         }
        
//         // Encryption methods
//         private static string EncryptString(string plainText)
//         {
//             if (string.IsNullOrEmpty(plainText)) return string.Empty;
            
//             using (Aes aes = Aes.Create())
//             {
//                 aes.Key = EncryptionKey;
//                 aes.IV = EncryptionIV;
                
//                 ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                
//                 using (MemoryStream ms = new MemoryStream())
//                 {
//                     using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
//                     {
//                         using (StreamWriter sw = new StreamWriter(cs))
//                         {
//                             sw.Write(plainText);
//                         }
//                         return Convert.ToBase64String(ms.ToArray());
//                     }
//                 }
//             }
//         }
        
//         private static string DecryptString(string cipherText)
//         {
//             if (string.IsNullOrEmpty(cipherText)) return string.Empty;
            
//             using (Aes aes = Aes.Create())
//             {
//                 aes.Key = EncryptionKey;
//                 aes.IV = EncryptionIV;
                
//                 ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                
//                 byte[] cipherBytes = Convert.FromBase64String(cipherText);
                
//                 using (MemoryStream ms = new MemoryStream(cipherBytes))
//                 {
//                     using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
//                     {
//                         using (StreamReader sr = new StreamReader(cs))
//                         {
//                             return sr.ReadToEnd();
//                         }
//                     }
//                 }
//             }
//         }
        
//         private static byte[] GenerateEncryptionKey()
//         {
//             using (var deriveBytes = new Rfc2898DeriveBytes(
//                 "YourVeryStrongPasswordForEncryption123!",
//                 Encoding.UTF8.GetBytes("YourSaltValue"),
//                 10000,
//                 HashAlgorithmName.SHA256))
//             {
//                 return deriveBytes.GetBytes(32); // 256-bit key
//             }
//         }
        
//         private static byte[] GenerateEncryptionIV()
//         {
//             using (var deriveBytes = new Rfc2898DeriveBytes(
//                 "YourIVGenerationPassword456!",
//                 Encoding.UTF8.GetBytes("YourIVSalt"),
//                 10000,
//                 HashAlgorithmName.SHA256))
//             {
//                 return deriveBytes.GetBytes(16); // 128-bit IV
//             }
//         }
//     }
// }