namespace BistroBlaze_API.Models.DTO
{
    public class RegisterRequestDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        //for testing
        public string Role { get; set; }
    }
}
