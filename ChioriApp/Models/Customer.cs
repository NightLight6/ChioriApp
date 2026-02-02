using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChioriApp.Models
{
    [Table("customers")]
    public class Customer
    {
        [Key]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Column("last_name")]
        public string LastName { get; set; } = string.Empty;

        [Column("patronymic")]
        public string? Patronymic { get; set; }

        [Column("birth_date")]
        public DateTime? BirthDate { get; set; }

        [Column("registration_date")]
        public DateTime RegistrationDate { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}