namespace ChioriApp.Models
{
    public class OrderListItem
    {
        public int OrderId { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerFirstName { get; set; } = string.Empty;
        public string CustomerLastName { get; set; } = string.Empty;
        public string CustomerPatronymic { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string DeliveryAddress { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public int StatusId { get; set; }
        public int PaymentMethodId { get; set; }
        public int DeliveryMethodId { get; set; }
        public DateTime OrderDate { get; set; }
        public string ClientFullName =>
        string.IsNullOrWhiteSpace(CustomerPatronymic)? $"{CustomerFirstName} {CustomerLastName}".Trim() : $"{CustomerFirstName} {CustomerPatronymic} {CustomerLastName}".Trim();
    }
}