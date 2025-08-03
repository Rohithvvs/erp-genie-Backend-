using System.ComponentModel.DataAnnotations;

namespace ErpApi.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string ShopName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(15)]
        public string? GST { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }
    }
=======
///// cursor/develop-net-core-erp-backend-api-2f37
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPBackend.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
=======

namespace ERPBackend.Models
{
    public class User
    {
//// erp
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
//// cursor/develop-net-core-erp-backend-api-2f37
        [StringLength(255)]
=======
////erp
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ShopName { get; set; } = string.Empty;

        [Required]
///// cursor/develop-net-core-erp-backend-api-2f37
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
=======
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

////// erp
        [StringLength(15)]
        public string GST { get; set; } = string.Empty;

        [Required]
////// cursor/develop-net-core-erp-backend-api-2f37
=======
        [Phone]
//////erp
        [StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
///// cursor/develop-net-core-erp-backend-api-2f37

    public class UserRegistrationDto
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string ShopName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string GST { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class UserLoginDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string ShopName { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }
=======
////// erp
}