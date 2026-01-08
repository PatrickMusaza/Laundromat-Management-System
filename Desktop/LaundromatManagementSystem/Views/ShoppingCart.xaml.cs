using LaundromatManagementSystem.Models;
using System.Collections.ObjectModel;

namespace LaundromatManagementSystem.Views
{
    public partial class ShoppingCart : ContentView
    {
        public ObservableCollection<CartItem> CartItems { get; } = new();
        private decimal _subtotal = 0;
        private decimal _tax = 0;
        private decimal _total = 0;
        
        public ShoppingCart()
        {
            InitializeComponent();
            CartItemsCollection.ItemsSource = CartItems;
        }
        
        public void AddItem(CartItem item)
        {
            // Check if item already exists
            var existingItem = CartItems.FirstOrDefault(i => i.Name == item.Name);
            
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                CartItems.Add(item);
            }
            
            UpdateCartDisplay();
        }
        
        private void OnRemoveItemTapped(object sender, TappedEventArgs e)
        {
            if (e.Parameter is string itemId)
            {
                var item = CartItems.FirstOrDefault(i => i.Id == itemId);
                if (item != null)
                {
                    CartItems.Remove(item);
                    UpdateCartDisplay();
                }
            }
        }
        
        private void UpdateCartDisplay()
        {
            // Calculate totals
            _subtotal = CartItems.Sum(item => item.TotalPrice);
            _tax = Math.Round(_subtotal * 0.18m, 2);
            _total = _subtotal + _tax;
            
            // Update labels
            SubtotalLabel.Text = $"{_subtotal:N0} RWF";
            TaxLabel.Text = $"{_tax:N0} RWF";
            TotalLabel.Text = $"{_total:N0} RWF";
            
            // Show/hide totals and payment button
            bool hasItems = CartItems.Count > 0;
            TotalsSection.IsVisible = hasItems;
            ProcessPaymentButton.IsVisible = hasItems;
        }
        
        private void OnProcessPaymentClicked(object sender, EventArgs e)
        {
            // Find the parent Dashboard
            var parent = this.Parent;
            while (parent != null && parent is not Dashboard)
            {
                parent = parent.Parent;
            }
            
            if (parent is Dashboard dashboard && dashboard.BindingContext is ViewModels.DashboardViewModel vm)
            {
                vm.ProcessPaymentCommand.Execute(null);
            }
        }
        
        // Public method to clear cart (called from Dashboard)
        public void ClearCart()
        {
            CartItems.Clear();
            UpdateCartDisplay();
        }
    }
}