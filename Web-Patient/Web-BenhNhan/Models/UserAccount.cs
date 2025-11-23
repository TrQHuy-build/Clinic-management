using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_BenhNhan.Models
{
    [Table("UserAccount")]
    public class UserAccount
    {
        [Key]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("fullname")]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Column("phone")]
        [MaxLength(15)]
        public string Phone { get; set; } = string.Empty;

        [Column("email")]
        [MaxLength(255)]
        public string? Email { get; set; }

        [Required]
        [Column("password_hash")]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [Column("role")]
        [MaxLength(20)]
        public string Role { get; set; } = "patient";

        [Column("status")]
        [MaxLength(20)]
        public string? Status { get; set; } = "active";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
