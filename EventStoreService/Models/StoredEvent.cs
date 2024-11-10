namespace EventStoreService.Models
{
    public class StoredEvent
    {
        public int Id { get; set; }
        public string EventType { get; set; }
        public string Data { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
