using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;          
using Microsoft.AspNetCore.Authorization;     
using GameReviewSystem.Data;                   
using GameReviewSystem.Models;                 
using GameReviewSystem.Services;               
using System.Threading.Tasks;                 

namespace GameReviewSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // This will map to /api/auth
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;

        // The constructor receives AppDbContext and JwtService via dependency injection.
        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // POST /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            // Check if the username already exists in the database.
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Username is already taken.");

            // Hash the provided password using BCrypt.
            var hashed = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Create a new User entity with the provided details.
            var user = new User
            {
                Username = dto.Username,
                PasswordHash = hashed,
                Email = dto.Email,              // Email from the DTO
                CreatedAt = DateTime.UtcNow     // Set the CreatedAt property to the current UTC time
            };

            // Add the new user to the database context and save changes.
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Return a 200 OK response with a success message.
            return Ok("User registered successfully.");
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // Look up the user by username.
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
            if (user == null)
                return BadRequest("Invalid credentials.");

            // Verify that the provided password matches the stored hash.
            bool valid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!valid)
                return BadRequest("Invalid credentials.");

            // Generate a JWT token for the authenticated user.
            var token = _jwtService.GenerateToken(user);

            // Return the token in a JSON response.
            return Ok(new { token });
        }

        // GET /api/auth/protected
        // This endpoint is protected by the [Authorize] attribute, meaning a valid JWT token must be provided.
        [Authorize]
        [HttpGet("protected")]
        public IActionResult ProtectedEndpoint()
        {
            return Ok("Hello, authorized user!");
        }

        // DTOs for register/login, defined inside the controller for convenience.
        public class RegisterDto
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }

        public class LoginDto
        {
            public string Username { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
}
