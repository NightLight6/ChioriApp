namespace ChioriApp.Models
{
    public class BasketItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;

        public decimal Total => Price * Quantity;
    }
}