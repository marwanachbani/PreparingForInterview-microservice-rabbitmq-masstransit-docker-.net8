namespace UserService.Dto
{
    public class UserDto
    {
        public UserDto(string username, string role)
        {
            Username = username;
            Role = role;
        }

        public string Username { get; set; }
        public string Role { get; set; }
    }
}
