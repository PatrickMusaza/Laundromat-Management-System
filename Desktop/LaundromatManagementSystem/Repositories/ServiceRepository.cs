using LaundromatManagementSystem.Data;
using LaundromatManagementSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace LaundromatManagementSystem.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly AppDbContext _context;

        public ServiceRepository(AppDbContext context)
        {
            _context = context;
        }

        public ServiceRepository()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("Data Source=laundromat.db;Password=SecurePassword123!")
                .Options;
            _context = new AppDbContext(options);
        }

        public async Task SeedDatabaseAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            
            // Check if database is empty and seed if needed
            if (!await _context.Services.AnyAsync())
            {
                // Database will be seeded from OnModelCreating
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Service>> GetServicesByCategoryAsync(string category, string languageCode = "EN")
        {
            return await _context.Services
                .Include(s => s.Category)
                .Where(s => s.Type.ToLower() == category.ToLower() && s.IsAvailable)
                .OrderBy(s => s.Price)
                .ToListAsync();
        }

        public async Task<Service?> GetServiceByIdAsync(int id)
        {
            return await _context.Services
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Service>> GetAllServicesAsync()
        {
            return await _context.Services
                .Include(s => s.Category)
                .Where(s => s.IsAvailable)
                .OrderBy(s => s.Type)
                .ThenBy(s => s.Price)
                .ToListAsync();
        }

        public async Task AddServiceAsync(Service service)
        {
            service.CreateDate = DateTime.UtcNow;
            service.UpdateDate = DateTime.UtcNow;
            
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateServiceAsync(Service service)
        {
            service.UpdateDate = DateTime.UtcNow;
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteServiceAsync(int id)
        {
            var service = await GetServiceByIdAsync(id);
            if (service != null)
            {
                // Soft delete by marking as unavailable
                service.IsAvailable = false;
                service.UpdateDate = DateTime.UtcNow;
                await UpdateServiceAsync(service);
            }
        }
    }
}