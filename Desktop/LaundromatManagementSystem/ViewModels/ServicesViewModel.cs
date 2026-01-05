using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LaundromatManagementSystem.Models;
using LaundromatManagementSystem.Services;

namespace LaundromatManagementSystem.ViewModels
{
    public partial class ServicesViewModel : ObservableObject
    {
        private readonly IServiceService _serviceService;
        private readonly ICartService _cartService;
        
        [ObservableProperty]
        private List<Service> _services;
        
        [ObservableProperty]
        private List<Service> _washServices;
        
        [ObservableProperty]
        private List<Service> _dryServices;
        
        [ObservableProperty]
        private List<Service> _addOnServices;
        
        [ObservableProperty]
        private List<Service> _packageServices;
        
        public ServicesViewModel(IServiceService serviceService, ICartService cartService)
        {
            _serviceService = serviceService;
            _cartService = cartService;
            LoadServices();
        }
        
        private async void LoadServices()
        {
            Services = await _serviceService.GetServicesAsync();
            WashServices = await _serviceService.GetServicesByTypeAsync(ServiceType.Wash);
            DryServices = await _serviceService.GetServicesByTypeAsync(ServiceType.Dry);
            AddOnServices = await _serviceService.GetServicesByTypeAsync(ServiceType.AddOn);
            PackageServices = await _serviceService.GetServicesByTypeAsync(ServiceType.Package);
        }
        
        [RelayCommand]
        private void AddToCart(Service service)
        {
            if (service != null)
            {
                _cartService.AddToCart(service);
            }
        }
    }
}