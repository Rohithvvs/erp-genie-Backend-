using System.ComponentModel.DataAnnotations;

namespace ErpApi.DTOs
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string ShopName { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? GST { get; set; }
        public string? PhoneNumber { get; set; }
    }
}