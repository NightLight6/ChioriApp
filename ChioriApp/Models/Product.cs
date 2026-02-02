using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChioriApp.Models
{
    [Table("products")]
    public class Product
    {
        [Key]
        [Column("product_id")]
        public int ProductId { get; set; }

        [Required, StringLength(50)]
        [Column("article")]
        public string Article { get; set; } = string.Empty;

        [Required, StringLength(200)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("manufacturer_id")]
        public int ManufacturerId { get; set; }

        [Column("supplier_id")]
        public int SupplierId { get; set; }

        [Column("unit_id")]
        public int UnitId { get; set; }

        [Column("status_id")]
        public int StatusId { get; set; }

        [Column("recommended_price")]
        public decimal RecommendedPrice { get; set; }

        [Column("max_discount_percent")]
        public decimal? MaxDiscountPercent { get; set; }
    }
}