using LaundromatManagementSystem.Models;

namespace LaundromatManagementSystem.Services
{
    public interface ICartService
    {
        event EventHandler CartUpdated;
        List<CartItem> GetCartItems();
        void AddToCart(Service service);
        void RemoveFromCart(int serviceId);
        void ClearCart();
        decimal GetTotalAmount();
        int GetItemCount();
    }
}