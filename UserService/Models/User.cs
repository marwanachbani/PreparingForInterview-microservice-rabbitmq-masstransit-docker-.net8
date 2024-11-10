using Microsoft.EntityFrameworkCore;
using UserService.Data;

namespace UserService.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; } // Hashed password storage
        public string Role { get; set; } = "client";
    }
}
