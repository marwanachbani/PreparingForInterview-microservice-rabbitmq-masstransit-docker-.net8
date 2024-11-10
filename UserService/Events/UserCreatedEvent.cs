namespace UserService.Events
{
    public class UserCreatedEvent
    {
        public string Role { get; set; }
        public string Username { get; set; }
    }
}
