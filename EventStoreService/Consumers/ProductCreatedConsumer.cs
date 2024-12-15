using EventStoreService.Data;
using EventStoreService.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SharedLibrary.Events;

namespace EventStoreService.Consumers
{
    public class ProductCreatedConsumer : IConsumer<ProductCreatedEvent>
    {
        private readonly EventStoreDbContext _dbContext;
        private readonly ILogger<UserCreatedConsumer> _logger;

        public ProductCreatedConsumer(EventStoreDbContext dbContext, ILogger<UserCreatedConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
        {
            var storedEvent = new StoredEvent
            {
                EventType = nameof(UserCreatedEvent),
                Data = JsonConvert.SerializeObject(context.Message)
            };

            // Add the event to the database
            _dbContext.StoredEvents.Add(storedEvent);
            await _dbContext.SaveChangesAsync();
        }
    }
    
}
