using MassTransit;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using EventStoreService.Data;
using EventStoreService.Models;
using System.Threading.Tasks;

namespace EventStoreService.Consumers
{
    public class UserCreatedConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly EventStoreDbContext _dbContext;
        private readonly ILogger<UserCreatedConsumer> _logger;

        public UserCreatedConsumer(EventStoreDbContext dbContext, ILogger<UserCreatedConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            // Log the received event for debugging
            _logger.LogInformation("Received UserCreatedEvent: Username = {Username}, Role = {Role}", context.Message.Username, context.Message.Role);

            // Create a stored event for the database
            var storedEvent = new StoredEvent
            {
                EventType = nameof(UserCreatedEvent),
                Data = JsonConvert.SerializeObject(context.Message)
            };

            // Add the event to the database
            _dbContext.StoredEvents.Add(storedEvent);
            await _dbContext.SaveChangesAsync();

            // Log success
            _logger.LogInformation("Stored UserCreatedEvent for user {Username}", context.Message.Username);
        }
    }

    public class UserCreatedEvent
    {
        public string Role { get; set; }
        public string Username { get; set; }
    }
}
