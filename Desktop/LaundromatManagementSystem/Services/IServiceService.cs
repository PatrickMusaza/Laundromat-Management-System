using LaundromatManagementSystem.ViewModels;

namespace LaundromatManagementSystem.Services
{
    public interface IServiceService
    {
        Task<List<ServiceItem>> GetServicesByCategoryAsync(string category, Language language);
        Task<List<ServiceItem>> GetAllServicesAsync(Language language);
    }
}