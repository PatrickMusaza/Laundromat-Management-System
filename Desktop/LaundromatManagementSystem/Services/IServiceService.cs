using LaundromatManagementSystem.Models;

namespace LaundromatManagementSystem.Services
{
    public interface IServiceService
    {
        // Category methods
        Task<List<CategoryItem>> GetAllCategoriesAsync(Language language);
        Task<CategoryItem?> GetCategoryByTypeAsync(string type, Language language);

        // Existing service methods
        Task<List<ServiceItem>> GetServicesByCategoryAsync(string category, Language language);
        Task<ServiceItem?> GetServiceByIdAsync(string id, Language language);
        Task InitializeDatabaseAsync();
    }
}