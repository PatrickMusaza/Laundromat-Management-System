using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using LaundromatManagementSystem.Models;

namespace LaundromatManagementSystem.Services;

public class ApplicationStateService : INotifyPropertyChanged
{
    private static ApplicationStateService _instance;
    public static ApplicationStateService Instance => _instance ??= new ApplicationStateService();

    private Theme _currentTheme = Theme.Light;
    private Language _currentLanguage = Language.EN;
    private ObservableCollection<CartItem> _cartItems = new();

    public event PropertyChangedEventHandler PropertyChanged;

    public Theme CurrentTheme
    {
        get => _currentTheme;
        set
        {
            if (_currentTheme != value)
            {
                _currentTheme = value;
                OnPropertyChanged();
                // Notify all subscribers
                ThemeChanged?.Invoke(this, value);
            }
        }
    }

    public Language CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (_currentLanguage != value)
            {
                _currentLanguage = value;
                OnPropertyChanged();
                // Notify all subscribers
                LanguageChanged?.Invoke(this, value);
            }
        }
    }

    public ObservableCollection<CartItem> CartItems
    {
        get => _cartItems;
        set
        {
            _cartItems = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CartTotal));
            OnPropertyChanged(nameof(ItemCount));
        }
    }

    public decimal CartTotal => CartItems.Sum(item => item.TotalPrice);
    public int ItemCount => CartItems.Sum(item => item.Quantity);

    public event EventHandler<Theme> ThemeChanged;
    public event EventHandler<Language> LanguageChanged;
    public event EventHandler CartUpdated;

    public void AddToCart(CartItem item)
    {
        var existingItem = CartItems.FirstOrDefault(i => i.ServiceId == item.ServiceId);
        if (existingItem != null)
        {
            existingItem.Quantity += 1;
        }
        else
        {
            var newItem = new CartItem
            {
                Id = Guid.NewGuid().ToString(),
                ServiceId = item.ServiceId,
                Name = item.Name,
                Price = item.Price,
                Quantity = 1,
                Addons = new ObservableCollection<ServiceAddon>(
                    item.Addons ?? new ObservableCollection<ServiceAddon>())
            };

            CartItems.Add(newItem);
        }

        OnPropertyChanged(nameof(CartItems));
        OnPropertyChanged(nameof(CartTotal));
        OnPropertyChanged(nameof(ItemCount));
        CartUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateQuantity(string itemId, int changeQuantity)
    {
        var item = CartItems.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            var newQuantity = item.Quantity + changeQuantity;
            if (newQuantity <= 0)
            {
                RemoveItem(item.Id);
            }
            else
            {
                item.Quantity = newQuantity;
                OnPropertyChanged(nameof(CartItems));
                OnPropertyChanged(nameof(CartTotal));
                OnPropertyChanged(nameof(ItemCount));
                CartUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void RemoveItem(string itemId)
    {
        var item = CartItems.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            CartItems.Remove(item);
            OnPropertyChanged(nameof(CartItems));
            OnPropertyChanged(nameof(CartTotal));
            OnPropertyChanged(nameof(ItemCount));
            CartUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    public void NotifyCartItemChanged(CartItem item)
    {
        // This will trigger the INotifyPropertyChanged on the item
        var index = CartItems.IndexOf(item);
        if (index >= 0)
        {
            // Replacing the item at the same index triggers collection change
            CartItems[index] = item;

            // Also notify totals changed
            OnPropertyChanged(nameof(CartTotal));
            OnPropertyChanged(nameof(ItemCount));
            CartUpdated?.Invoke(this, EventArgs.Empty);
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}