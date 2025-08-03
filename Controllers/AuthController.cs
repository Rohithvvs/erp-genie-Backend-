using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ErpApi.Data;
using ErpApi.DTOs;
using ErpApi.Models;
using ErpApi.Services;

namespace ErpApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly JwtTokenService _jwtTokenService;

        public AuthController(ApplicationDbContext db, IPasswordHasher<User> passwordHasher, JwtTokenService jwtTokenService)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
                return Conflict("Username already exists.");

            var user = new User
            {
                Username = dto.Username,
                ShopName = dto.ShopName,
                Address = dto.Address,
                GST = dto.GST,
                PhoneNumber = dto.PhoneNumber
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Registration successful" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null)
                return Unauthorized("Invalid credentials.");

            var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (verification == PasswordVerificationResult.Failed)
                return Unauthorized("Invalid credentials.");

            var token = _jwtTokenService.GenerateToken(user);
            return Ok(new { token });
        }
    }
}