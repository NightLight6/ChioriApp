using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChioriApp.Models
{
    [Table("order_items")]
    public class OrderItem
    {
        [Key]
        [Column("item_id")]
        public int ItemId { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("price_at_order")]
        public decimal PriceAtOrder { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; } = 0m;

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}