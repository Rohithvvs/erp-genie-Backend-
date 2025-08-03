using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ErpApi.Data;
using ErpApi.DTOs;
using ErpApi.Models;
using ErpApi.Services;

namespace ErpApi.Controllers
=======
using ERPBackend.Data;
using ERPBackend.Models;
using ERPBackend.Services;
using Microsoft.EntityFrameworkCore;
=======
using Microsoft.EntityFrameworkCore;
using ERPBackend.Data;
using ERPBackend.Models;
using ERPBackend.DTOs;
using ERPBackend.Services;
using BCrypt.Net;

namespace ERPBackend.Controllers
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
=======
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthController(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="registrationDto">User registration details</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if username already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == registrationDto.Username);

            if (existingUser != null)
            {
                return BadRequest(new { message = "Username already exists" });
            }

            // Hash password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password);

            // Create new user
            var user = new User
            {
                Username = registrationDto.Username,
                PasswordHash = passwordHash,
                ShopName = registrationDto.ShopName,
                Address = registrationDto.Address,
                GST = registrationDto.GST,
                PhoneNumber = registrationDto.PhoneNumber
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);

            var response = new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                ShopName = user.ShopName,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            return Ok(response);
        }

        /// <summary>
        /// Authenticate user and get JWT token
        /// </summary>
        /// <param name="loginDto">User login credentials</param>
        /// <returns>Authentication response with JWT token</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 401)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find user by username
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);

            var response = new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                ShopName = user.ShopName,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            return Ok(response);
=======
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto registerDto)
        {
            try
            {
                // Check if user already exists
                if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
                {
                    return BadRequest(new { message = "Username already exists" });
                }

                // Hash the password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

                // Create new user
                var user = new User
                {
                    Username = registerDto.Username,
                    PasswordHash = passwordHash,
                    ShopName = registerDto.ShopName,
                    Address = registerDto.Address,
                    GST = registerDto.GST,
                    PhoneNumber = registerDto.PhoneNumber
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Generate JWT token
                var token = _jwtService.GenerateToken(user.Username, user.Id);
                var expiresAt = DateTime.UtcNow.AddHours(24);

                var response = new AuthResponseDto
                {
                    Token = token,
                    Username = user.Username,
                    ShopName = user.ShopName,
                    ExpiresAt = expiresAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
        {
            try
            {
                // Find user by username
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

                if (user == null)
                {
                    return BadRequest(new { message = "Invalid username or password" });
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    return BadRequest(new { message = "Invalid username or password" });
                }

                // Generate JWT token
                var token = _jwtService.GenerateToken(user.Username, user.Id);
                var expiresAt = DateTime.UtcNow.AddHours(24);

                var response = new AuthResponseDto
                {
                    Token = token,
                    Username = user.Username,
                    ShopName = user.ShopName,
                    ExpiresAt = expiresAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
            }
        }

        [HttpGet("validate")]
        public IActionResult ValidateToken()
        {
            var token = Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
            
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new { message = "Token is required" });
            }

            var principal = _jwtService.ValidateToken(token);
            
            if (principal == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            return Ok(new { message = "Token is valid", username = principal.Identity?.Name });
        }
    }
}