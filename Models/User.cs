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
}