// Models/CartItem.cs
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LaundromatManagementSystem.Models
{
    public class CartItem : INotifyPropertyChanged
    {
        private string _id = string.Empty;
        private string _serviceId = string.Empty;
        private string _serviceType;
        private string _name = string.Empty;
        private decimal _price;
        private int _quantity = 1;
        private ObservableCollection<ServiceAddon> _addons = new();

        public string Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ServiceId
        {
            get => _serviceId;
            set
            {
                if (_serviceId != value)
                {
                    _serviceId = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ServiceType
        {
            get => _serviceType;
            set
            {
                if (_serviceType != value)
                {
                    _serviceType = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        public ObservableCollection<ServiceAddon> Addons
        {
            get => _addons;
            set
            {
                _addons = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        public decimal TotalPrice => (Price + Addons.Sum(a => a.Price)) * Quantity;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ServiceAddon : INotifyPropertyChanged
    {
        private string _name = string.Empty;
        private decimal _price;

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                if (_price != value)
                {
                    _price = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}