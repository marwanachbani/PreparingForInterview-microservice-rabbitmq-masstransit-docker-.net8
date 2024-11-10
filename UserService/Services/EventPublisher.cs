using MassTransit;
using MassTransit.Transports;

namespace UserService.Services
{
    public class EventPublisher : IEventPublisher
    {
        
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly ILogger<EventPublisher> _logger;

        public EventPublisher(ISendEndpointProvider sendEndpointProvider, ILogger<EventPublisher> logger)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _logger = logger;
        }

        public async Task SendEventToQueueAsync<T>(T eventMessage, string queueName) where T : class
        {
            try
            {
                var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}"));
                await endpoint.Send(eventMessage);
                _logger.LogInformation("Event sent to queue {QueueName}: {EventMessage}", queueName, eventMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending event to queue {QueueName}", queueName);
            }

        }
    }
}
