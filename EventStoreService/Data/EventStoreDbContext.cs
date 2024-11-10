using EventStoreService.Models;
using Microsoft.EntityFrameworkCore;

namespace EventStoreService.Data
{
    public class EventStoreDbContext : DbContext
    {
        public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options) : base(options) { }

        public DbSet<StoredEvent> StoredEvents { get; set; }
    }
}
