using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserService.Data;
using UserService.Models;
using BCrypt.Net;
using UserService.Dto;
using UserService.Events;
using UserService.Services;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserDbContext _context;
        private readonly IEventPublisher _eventPublisher;
        private readonly IConfiguration _configuration;

        public UsersController(UserDbContext context, IConfiguration configuration , IEventPublisher eventPublisher)
        {
            _context = context;
            _configuration = configuration;
            _eventPublisher = eventPublisher;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Register(User user)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.Role = "client"; // Default to "client" role
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var userCreatedEvent = new UserCreatedEvent { Role = user.Role, Username = user.Username };
            await _eventPublisher.SendEventToQueueAsync(userCreatedEvent,"usercreated");
            return "welcome " + user.Username; 
            
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string userName , string password)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == userName);

            if (existingUser == null || !BCrypt.Net.BCrypt.Verify(password, existingUser.PasswordHash))
            {
                return Unauthorized("Invalid username or password");
            }

            var token = GenerateJwtToken(existingUser);
            return Ok(new { token });
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            var list =  await _context.Users.ToListAsync();
            var results = new List<UserDto>();
            foreach (var item in list)
            {
                results.Add(new UserDto(item.Username, item.Role));
            }
            return results;
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
