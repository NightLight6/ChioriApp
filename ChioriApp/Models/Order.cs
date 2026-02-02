using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChioriApp.Models
{
    [Table("orders")]
    public class Order
    {
        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }

        [Required, StringLength(20)]
        [Column("order_number")]
        public string OrderNumber { get; set; } = string.Empty;

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("delivery_method_id")]
        public int DeliveryMethodId { get; set; }

        [Required]
        [Column("delivery_address")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Required]
        [Column("contact_name")]
        public string ContactName { get; set; } = string.Empty;

        [Required]
        [Column("contact_phone")]
        public string ContactPhone { get; set; } = string.Empty;

        [Required]
        [Column("contact_email")]
        public string ContactEmail { get; set; } = string.Empty;

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }

        [Column("final_amount")]
        public decimal FinalAmount { get; set; }

        [Column("payment_method_id")]
        public int PaymentMethodId { get; set; }

        [Column("order_date")]
        public DateTime OrderDate { get; set; }

        [Column("pickup_code")]
        public string? PickupCode { get; set; }

        [Column("comments")]
        public string? Comments { get; set; }

        [Column("planned_delivery_date")]
        public DateTime? PlannedDeliveryDate { get; set; }

        [Column("actual_delivery_date")]
        public DateTime? ActualDeliveryDate { get; set; }

        [Column("discount_amount")]
        public decimal DiscountAmount { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}