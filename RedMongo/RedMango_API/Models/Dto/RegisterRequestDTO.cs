namespace RedMango_API.Models.Dto
{
    public class RegisterRequestDTO
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User";
    }
}
