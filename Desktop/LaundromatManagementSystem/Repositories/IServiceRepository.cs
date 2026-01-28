using LaundromatManagementSystem.Entities;

namespace LaundromatManagementSystem.Repositories
{
    public interface IServiceRepository
    {
        Task<List<Service>> GetServicesByCategoryAsync(string category, string languageCode = "EN");
        Task<Service?> GetServiceByIdAsync(int id);
        Task<List<Service>> GetAllServicesAsync();
        Task AddServiceAsync(Service service);
        Task UpdateServiceAsync(Service service);
        Task DeleteServiceAsync(int id);
        Task SeedDatabaseAsync();
    }
}