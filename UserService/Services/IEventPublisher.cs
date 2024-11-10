namespace UserService.Services
{
    public interface IEventPublisher
    {
        Task SendEventToQueueAsync<T>(T eventMessage, string queueName) where T : class;
    }
}
