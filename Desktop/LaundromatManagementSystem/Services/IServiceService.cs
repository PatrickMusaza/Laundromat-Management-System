using LaundromatManagementSystem.Models;

namespace LaundromatManagementSystem.Services
{
    public interface IServiceService
    {
        Task<List<ServiceItem>> GetServicesByCategoryAsync(string category, Language language);
        Task<ServiceItem?> GetServiceByIdAsync(string id, Language language);
        Task InitializeDatabaseAsync();
    }
}