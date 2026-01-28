using LaundromatManagementSystem.Models;
using LaundromatManagementSystem.Repositories;

namespace LaundromatManagementSystem.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _repository;

        public ServiceService(IServiceRepository repository)
        {
            _repository = repository;
        }

        public async Task InitializeDatabaseAsync()
        {
            await _repository.SeedDatabaseAsync();
        }

        public async Task<List<ServiceItem>> GetServicesByCategoryAsync(string category, Language language)
        {
            var services = await _repository.GetServicesByCategoryAsync(category);
            return services.Select(s => ServiceItem.FromEntity(s, (Models.Language)language)).ToList();
        }

        public async Task<ServiceItem?> GetServiceByIdAsync(string id, Language language)
        {
            if (int.TryParse(id, out int serviceId))
            {
                var service = await _repository.GetServiceByIdAsync(serviceId);
                if (service != null)
                {
                    return ServiceItem.FromEntity(service, (Models.Language)language);
                }
            }
            return null;
        }

        public async Task<List<CategoryItem>> GetAllCategoriesAsync(Language language)
        {
            return await _repository.GetCategoryItemsAsync(language);
        }

        public async Task<CategoryItem?> GetCategoryByTypeAsync(string type, Language language)
        {
            var category = await _repository.GetCategoryByTypeAsync(type);
            if (category != null)
            {
                return CategoryItem.FromEntity(category, (Models.Language)language);
            }
            return null;
        }
    }
}