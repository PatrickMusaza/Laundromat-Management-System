using LaundromatManagementSystem.Models;

namespace LaundromatManagementSystem.Services
{
    public interface ICartService
    {
        event EventHandler CartUpdated;
        IReadOnlyList<CartItem> GetCartItems();
        void AddToCart(CartItem item);
        void RemoveFromCart(string itemId);
        void UpdateQuantity(string itemId, int quantity);
        void ClearCart();
        decimal GetTotalAmount();
        int GetItemCount();
        void AddItem(CartItem item);
        void RemoveItem(string itemId);
    }
}