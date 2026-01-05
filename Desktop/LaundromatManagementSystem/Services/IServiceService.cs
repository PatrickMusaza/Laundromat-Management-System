using LaundromatManagementSystem.Models;

namespace LaundromatManagementSystem.Services
{
    public interface IServiceService
    {
        Task<List<Service>> GetServicesAsync();
        Task<List<Service>> GetServicesByTypeAsync(ServiceType type);
        Task<Service?> GetServiceByIdAsync(int id);
    }
}
