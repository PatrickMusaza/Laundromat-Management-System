using LaundromatManagementSystem.Entities;
using LaundromatManagementSystem.Models;

namespace LaundromatManagementSystem.Repositories
{
    public interface IServiceRepository
    {
        // Category methods
        Task<List<ServiceCategory>> GetAllCategoriesAsync();
        Task<ServiceCategory?> GetCategoryByTypeAsync(string type);
        Task<List<CategoryItem>> GetCategoryItemsAsync(Language language);

        // Existing service methods
        Task<List<Service>> GetServicesByCategoryAsync(string category, string languageCode = "EN");
        Task<Service?> GetServiceByIdAsync(int id);
        Task<List<Service>> GetAllServicesAsync();
        Task AddServiceAsync(Service service);
        Task UpdateServiceAsync(Service service);
        Task DeleteServiceAsync(int id);
        Task SeedDatabaseAsync();
    }
}