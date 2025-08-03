using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ERPBackend.Data;
using ERPBackend.DTOs;
using ERPBackend.Models;
using ERPBackend.Services;
using BCrypt.Net;

namespace ERPBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthController(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if username already exists
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest(new { message = "Username already exists" });
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // Create user
            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                ShopName = request.ShopName,
                Address = request.Address,
                GST = request.GST,
                PhoneNumber = request.PhoneNumber
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate token
            var token = _jwtService.GenerateToken(user);

            var response = new AuthResponse
            {
                Token = token,
                Username = user.Username,
                ShopName = user.ShopName,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find user by username
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid username or password" });
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest(new { message = "Invalid username or password" });
            }

            // Generate token
            var token = _jwtService.GenerateToken(user);

            var response = new AuthResponse
            {
                Token = token,
                Username = user.Username,
                ShopName = user.ShopName,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            return Ok(response);
        }
    }
}