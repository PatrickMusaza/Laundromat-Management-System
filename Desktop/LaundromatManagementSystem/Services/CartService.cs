using LaundromatManagementSystem.Models;

namespace LaundromatManagementSystem.Services
{
    public class CartService : ICartService
    {
        private List<CartItem> _cartItems = new();

        public event EventHandler? CartUpdated;

        public IReadOnlyList<CartItem> GetCartItems() => _cartItems;

        public void AddToCart(CartItem item)
        {
            var existing = _cartItems.FirstOrDefault(i =>
                i.Name == item.Name &&
                i.Addons.SequenceEqual(item.Addons, new AddonComparer()));

            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                _cartItems.Add(new CartItem
                {
                    Id = item.Id,
                    Name = item.Name,
                    Price = item.Price,
                    Addons = new System.Collections.ObjectModel.ObservableCollection<ServiceAddon>(item.Addons),
                    Quantity = 1
                });
            }

            CartUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void AddItem(CartItem item)
        {
            _cartItems.Add(item);
            CartUpdated?.Invoke(this, EventArgs.Empty);
        }

        public void RemoveItem(string itemId)
        {
            var item = _cartItems.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                _cartItems.Remove(item);
                CartUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public void RemoveFromCart(string itemId)
        {
            var item = _cartItems.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                _cartItems.Remove(item);
                CartUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public void UpdateQuantity(string itemId, int quantity)
        {
            if (quantity <= 0)
            {
                RemoveFromCart(itemId);
                return;
            }

            var item = _cartItems.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                item.Quantity = quantity;
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

        private class AddonComparer : IEqualityComparer<ServiceAddon>
        {
            public bool Equals(ServiceAddon x, ServiceAddon y)
            {
                if (x == null && y == null) return true;
                if (x == null || y == null) return false;
                return x.Name == y.Name && x.Price == y.Price;
            }

            public int GetHashCode(ServiceAddon obj) =>
                HashCode.Combine(obj.Name, obj.Price);
        }
    }
}