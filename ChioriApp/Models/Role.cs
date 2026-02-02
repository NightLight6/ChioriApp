using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChioriApp.Models
{
    [Table("roles")]
    public class Role
    {
        [Key]
        [Column("role_id")]
        public int RoleId { get; set; }

        [Required, StringLength(50)]
        [Column("role_name")]
        public string RoleName { get; set; } = string.Empty;

        [Column("access_level")]
        public int AccessLevel { get; set; }
    }
}