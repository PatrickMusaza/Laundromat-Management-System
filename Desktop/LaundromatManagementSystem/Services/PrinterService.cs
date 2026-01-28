using LaundromatManagementSystem.Models;
using System.Text;

namespace LaundromatManagementSystem.Services
{
    public interface IPrinterService
    {
        Task<bool> PrintReceipt(ReceiptContent receipt);
        bool IsPrinterAvailable { get; }
    }

    public class ReceiptContent
    {
        public string TransactionId { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Cashier { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Change { get; set; }
        public decimal Tax { get; set; }
        public decimal Subtotal { get; set; }
        public decimal GrandTotal { get; set; }
        public List<CartItem> Items { get; set; } = new();
        public string BusinessName { get; set; } = "LAUNDROMAT SYSTEM";
        public string BusinessAddress { get; set; } = "Kigali, Rwanda";
        public string BusinessPhone { get; set; } = "+250 788 123 456";
    }

    public class PrinterService : IPrinterService
    {
        public bool IsPrinterAvailable => CheckPrinterAvailability();

        public async Task<bool> PrintReceipt(ReceiptContent receipt)
        {
            try
            {
                // For now, just simulate printing and show info to user
                // In real implementation, this would connect to thermal printer
                
                var receiptText = GenerateReceiptText(receipt);
                
                // Save receipt to file for debugging
                await SaveReceiptToFile(receiptText);
                
                // Show receipt preview to user
                await ShowReceiptPreview(receiptText);
                
                // Log the receipt
                Console.WriteLine($"Receipt generated for Transaction: {receipt.TransactionId}");
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Printer error: {ex.Message}");
                throw;
            }
        }

        private string GenerateReceiptText(ReceiptContent receipt)
        {
            var sb = new StringBuilder();
            
            // Header
            sb.AppendLine("========================================");
            sb.AppendLine($"      {receipt.BusinessName}");
            sb.AppendLine($"      {receipt.BusinessAddress}");
            sb.AppendLine($"      {receipt.BusinessPhone}");
            sb.AppendLine("========================================");
            sb.AppendLine($"Date: {receipt.Date:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Transaction: {receipt.TransactionId}");
            sb.AppendLine($"Cashier: {receipt.Cashier}");
            
            if (!string.IsNullOrEmpty(receipt.CustomerPhone))
                sb.AppendLine($"Customer: {receipt.CustomerPhone}");
            
            sb.AppendLine("----------------------------------------");
            
            // Items
            sb.AppendLine("ITEMS:");
            foreach (var item in receipt.Items)
            {
                sb.AppendLine($"{item.Name}");
                sb.AppendLine($"  {item.Quantity} x {item.Price:N0} RWF = {item.TotalPrice:N0} RWF");
                
                if (item.Addons?.Count > 0)
                {
                    foreach (var addon in item.Addons)
                    {
                        sb.AppendLine($"  + {addon.Name}: {addon.Price:N0} RWF");
                    }
                }
            }
            
            sb.AppendLine("----------------------------------------");
            
            // Totals
            sb.AppendLine($"SUBTOTAL: {receipt.Items.Sum(i => i.TotalPrice):N0} RWF");
            sb.AppendLine($"TAX (10%): {receipt.Items.Sum(i => i.TotalPrice) * 0.1m:N0} RWF");
            sb.AppendLine($"TOTAL: {receipt.Amount:N0} RWF");
            sb.AppendLine($"PAYMENT METHOD: {receipt.PaymentMethod}");
            
            if (receipt.PaymentMethod == "Cash" && receipt.Change > 0)
                sb.AppendLine($"CHANGE: {receipt.Change:N0} RWF");
            
            // Footer
            sb.AppendLine("----------------------------------------");
            sb.AppendLine("THANK YOU FOR YOUR BUSINESS!");
            sb.AppendLine("Please keep this receipt for your records");
            sb.AppendLine("========================================");
            
            return sb.ToString();
        }

        private async Task SaveReceiptToFile(string receiptText)
        {
            try
            {
                var receiptsFolder = Path.Combine(FileSystem.AppDataDirectory, "Receipts");
                Directory.CreateDirectory(receiptsFolder);
                
                var fileName = $"Receipt_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                var filePath = Path.Combine(receiptsFolder, fileName);
                
                await File.WriteAllTextAsync(filePath, receiptText);
                Console.WriteLine($"Receipt saved to: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save receipt: {ex.Message}");
            }
        }

        private async Task ShowReceiptPreview(string receiptText)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Receipt Generated",
                    $"Receipt has been generated and is ready for printing.\n\n" +
                    $"If connected to a thermal printer, receipt would now print.\n\n" +
                    $"Receipt has been saved to app data folder.",
                    "OK");
            });
        }

        private bool CheckPrinterAvailability()
        {
            // For now, always return true for simulation
            // In real implementation, check Bluetooth/USB printer connectivity
            return true;
        }
    }
}