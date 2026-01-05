using LaundromatManagementSystem.Models;

namespace LaundromatManagementSystem.Services
{
    public class CartService : ICartService
    {
        private List<CartItem> _cartItems = new();
        
        public event EventHandler CartUpdated;
        
        public List<CartItem> GetCartItems() => _cartItems;
        
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
        
        public void RemoveFromCart(int serviceId)
        {
            var item = _cartItems.FirstOrDefault(item => item.Service.Id == serviceId);
            if (item != null)
            {
                _cartItems.Remove(item);
                CartUpdated?.Invoke(this, EventArgs.Empty);
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

        public void UpdateQuantity(string itemId, int quantity)
        {
            var item = _cartItems.FirstOrDefault(i => i.Service.Id.ToString() == itemId);
            if (item != null)
            {
                item.Quantity = quantity;
                CartUpdated?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}