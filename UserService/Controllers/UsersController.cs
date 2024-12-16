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

        [Authorize(Policy = "AdminOnly")]
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

        private string GenerateJwtToken(User user)
        {
            var jwtConfig = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("Role", user.Role) 
            };

            var token = new JwtSecurityToken(
                issuer: jwtConfig["Issuer"],
                audience: jwtConfig["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtConfig["ExpirationMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
