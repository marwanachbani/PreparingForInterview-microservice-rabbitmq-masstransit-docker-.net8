using BCrypt.Net;
using UserService.Models;

namespace UserService.Data
{
    public static class UserDbInitializer
    {
        public static void Initialize(UserDbContext context)
        {
            context.Database.EnsureCreated();
            if (!context.Users.Any())
            {
                var adminUser = new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("AdminPassword123"), // Securely hash the password
                    Role = "admin"
                };

                context.Users.Add(adminUser);
                context.SaveChanges();
            }
        }
    }
}
