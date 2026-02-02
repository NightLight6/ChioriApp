namespace ChioriApp.Models
{
    public class OrderListItem
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public int StatusId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal FinalAmount { get; set; }
    }
}