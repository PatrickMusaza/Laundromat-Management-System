using LaundromatManagementSystem.Models;
using System.Linq;

namespace LaundromatManagementSystem.Services
{
    public class ServiceService : IServiceService
    {
        private List<Service> _services = new()
        {
            new Service 
            { 
                Id = 1, 
                Name = "Hot Water Wash", 
                Type = ServiceType.Wash, 
                Price = 5000,
                Icon = "🔥",
                Description = "Hot water wash with detergent"
            },
            new Service 
            { 
                Id = 2, 
                Name = "Cold Water Wash", 
                Type = ServiceType.Wash, 
                Price = 6000,
                Icon = "💧",
                Description = "Cold water wash with detergent"
            },
            new Service 
            { 
                Id = 3, 
                Name = "Express Wash", 
                Type = ServiceType.Wash, 
                Price = 8000,
                Icon = "⚡",
                Description = "Fast wash service (30 minutes)"
            },
            new Service 
            { 
                Id = 4, 
                Name = "Dry Cleaning", 
                Type = ServiceType.Dry, 
                Price = 10000,
                Icon = "🌀",
                Description = "Professional dry cleaning"
            },
            new Service 
            { 
                Id = 5, 
                Name = "Ironing", 
                Type = ServiceType.AddOn, 
                Price = 3000,
                Icon = "👔",
                Description = "Clothes ironing service"
            },
            new Service 
            { 
                Id = 6, 
                Name = "Premium Package", 
                Type = ServiceType.Package, 
                Price = 15000,
                Icon = "⭐",
                Description = "Wash + Dry + Ironing package"
            }
        };
        
        public Task<List<Service>> GetServicesAsync()
        {
            return Task.FromResult(_services);
        }
        
        public Task<List<Service>> GetServicesByTypeAsync(ServiceType type)
        {
            var filtered = _services.Where(s => s.Type == type).ToList();
            return Task.FromResult(filtered);
        }
        
        public Task<Service?> GetServiceByIdAsync(int id)
        {
            var service = _services.FirstOrDefault(s => s.Id == id);
            return Task.FromResult(service);
        }
    }
}