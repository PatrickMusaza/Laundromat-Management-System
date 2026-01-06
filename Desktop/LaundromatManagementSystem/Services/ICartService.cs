using LaundromatManagementSystem.Models;
using System.Collections.ObjectModel;

namespace LaundromatManagementSystem.Services
{
    public interface ICartService
    {
        event EventHandler? CartUpdated;
        ObservableCollection<CartItem> GetCartItems();
        void AddToCart(Service service);
        void RemoveFromCart(string itemId);
        void UpdateQuantity(string itemId, int quantity);
        void ClearCart();
        decimal GetTotalAmount();
        int GetItemCount();
    }
}