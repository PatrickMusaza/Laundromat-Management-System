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

        public async Task SeedDatabaseAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            if (!await _context.Services.AnyAsync())
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Service>> GetServicesByCategoryAsync(string category, string languageCode = "EN")
        {
            // Fix: Load all services first, then order in memory
            var services = await _context.Services
                .Include(s => s.Category)
                .Where(s => s.Type.ToLower() == category.ToLower() && s.IsAvailable)
                .ToListAsync(); // Remove OrderBy from SQL query

            // Order in memory after loading
            return services.OrderBy(s => s.Price).ToList();
        }

        public async Task<Service?> GetServiceByIdAsync(int id)
        {
            return await _context.Services
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Service>> GetAllServicesAsync()
        {
            // Fix: Load all services first, then order in memory
            var services = await _context.Services
                .Include(s => s.Category)
                .Where(s => s.IsAvailable)
                .ToListAsync();

            // Order in memory
            return services
                .OrderBy(s => s.Type)
                .ThenBy(s => s.Price)
                .ToList();
        }

        // Alternative solution: Use double instead of decimal for ordering
        public async Task<List<Service>> GetServicesByCategoryAsync_Alternative(string category, string languageCode = "EN")
        {
            // Convert decimal to double for ordering in SQL
            return await _context.Services
                .Include(s => s.Category)
                .Where(s => s.Type.ToLower() == category.ToLower() && s.IsAvailable)
                .OrderBy(s => (double)s.Price) // Cast to double for SQLite
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
                service.IsAvailable = false;
                service.UpdateDate = DateTime.UtcNow;
                await UpdateServiceAsync(service);
            }
        }
    }
}