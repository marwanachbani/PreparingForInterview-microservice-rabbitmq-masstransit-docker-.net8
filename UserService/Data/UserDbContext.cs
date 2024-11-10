using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public override int SaveChanges()
        {
            SetClientRoleForNewUsers();
            return base.SaveChanges();
        }

        private void SetClientRoleForNewUsers()
        {
            foreach (var entry in ChangeTracker.Entries<User>().Where(e => e.State == EntityState.Added))
            {
                if (string.IsNullOrEmpty(entry.Entity.Role))
                {
                    entry.Entity.Role = "client";
                }
            }
        }
    }
}
