using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;       // For EF operations
using GameReviewSystem.Data;                       // Your EF DbContext
using GameReviewSystem.Models;                     // Your User entity
using GameReviewSystem.Services;                   // For JwtService
using System.Threading.Tasks;
using GameReviewSystem.Data;
using GameReviewSystem.Models;
using GameReviewSystem.Services;
using Microsoft.AspNetCore.Authorization;

namespace GameReviewSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // => /api/auth
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // POST /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // 1) Check if username is taken
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Username is already taken.");

            // 2) Hash password (using BCrypt)
            var hashed = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // 3) Create user
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = hashed
                // Optionally user.Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // 1) Lookup user by username
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null)
                return BadRequest("Invalid credentials.");

            // 2) Verify hashed password
            bool valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!valid)
                return BadRequest("Invalid credentials.");

            // 3) Generate JWT token
            var token = _jwtService.GenerateToken(user);

            return Ok(new { token });
        }

    [Authorize]
        [HttpGet("protected")]
        public IActionResult ProtectedEndpoint()
        {
            // If code runs here, the user is authenticated
            return Ok("Hello, authorized user!");
        }

    // DTOs for register/login
    public class RegisterDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    }
}