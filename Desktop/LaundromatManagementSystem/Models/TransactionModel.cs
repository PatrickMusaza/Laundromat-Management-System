namespace LaundromatManagementSystem.Models
{
    public class TransactionModel
    {
        public string TransactionId { get; set; } = string.Empty;
        public string Status { get; set; } = "pending";
        public string PaymentMethod { get; set; } = string.Empty;
        public double Subtotal { get; set; }
        public double TaxAmount { get; set; }
        public double TotalAmount { get; set; }
        public double? CashReceived { get; set; }
        public double? ChangeAmount { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerTin { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public List<TransactionItemModel> Items { get; set; } = new();
        public List<PaymentRecordModel> Payments { get; set; } = new();

        // Helper method to create from entity
        public static TransactionModel FromEntity(Entities.Transaction entity)
        {
            return new TransactionModel
            {
                TransactionId = entity.TransactionId,
                Status = entity.Status,
                PaymentMethod = entity.PaymentMethod,
                Subtotal = entity.Subtotal,
                TaxAmount = entity.TaxAmount,
                TotalAmount = entity.TotalAmount,
                CashReceived = entity.CashReceived,
                ChangeAmount = entity.ChangeAmount,
                CustomerName = entity.CustomerName,
                CustomerTin = entity.CustomerTin,
                CustomerPhone = entity.CustomerPhone,
                TransactionDate = entity.TransactionDate,
                Items = entity.Items.Select(TransactionItemModel.FromEntity).ToList(),
                Payments = entity.Payments.Select(PaymentRecordModel.FromEntity).ToList()
            };
        }
    }

    public class TransactionItemModel
    {
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? ServiceDescription { get; set; }
        public double UnitPrice { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public string ServiceIcon { get; set; } = string.Empty;

        public static TransactionItemModel FromEntity(Entities.TransactionItem entity)
        {
            return new TransactionItemModel
            {
                ServiceId = entity.ServiceId,
                ServiceName = entity.ServiceName,
                ServiceDescription = entity.ServiceDescription,
                UnitPrice = entity.UnitPrice,
                Quantity = entity.Quantity,
                TotalPrice = entity.TotalPrice,
                ServiceType = entity.ServiceType,
                ServiceIcon = entity.ServiceIcon
            };
        }
    }

    public class PaymentRecordModel
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = "pending";
        public double Amount { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? PaymentDetails { get; set; }
        public DateTime PaymentDate { get; set; }

        public static PaymentRecordModel FromEntity(Entities.PaymentRecord entity)
        {
            return new PaymentRecordModel
            {
                PaymentMethod = entity.PaymentMethod,
                Status = entity.Status,
                Amount = entity.Amount,
                ReferenceNumber = entity.ReferenceNumber,
                PaymentDetails = entity.PaymentDetails,
                PaymentDate = entity.PaymentDate
            };
        }
    }
}