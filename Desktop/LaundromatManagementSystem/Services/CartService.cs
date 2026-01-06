using LaundromatManagementSystem.Models;
using System.Collections.ObjectModel;

namespace LaundromatManagementSystem.Services
{
    public class CartService : ICartService
    {
        private ObservableCollection<CartItem> _cartItems = new();
        
        public event EventHandler? CartUpdated;
        
        public ObservableCollection<CartItem> GetCartItems() => _cartItems;
        
        public void AddToCart(Service service)
        {
            var existingItem = _cartItems.FirstOrDefault(item => item.Service.Id == service.Id);
            
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                _cartItems.Add(new CartItem { Service = service, Quantity = 1 });
            }
            
            CartUpdated?.Invoke(this, EventArgs.Empty);
        }
        
        public void RemoveFromCart(string itemId)
        {
            var item = _cartItems.FirstOrDefault(item => item.Id == itemId);
            if (item != null)
            {
                _cartItems.Remove(item);
                CartUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
        
        public void UpdateQuantity(string itemId, int quantity)
        {
            var item = _cartItems.FirstOrDefault(item => item.Id == itemId);
            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
                CartUpdated?.Invoke(this, EventArgs.Empty);
            }
            else if (item != null && quantity <= 0)
            {
                RemoveFromCart(itemId);
            }
        }
        
        public void ClearCart()
        {
            _cartItems.Clear();
            CartUpdated?.Invoke(this, EventArgs.Empty);
        }
        
        public decimal GetTotalAmount() => 
            _cartItems.Sum(item => item.TotalPrice);
            
        public int GetItemCount() => 
            _cartItems.Sum(item => item.Quantity);
    }
}