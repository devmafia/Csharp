
namespace UserService.Dtos
{
    public class CreateUserProfile
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Preferences { get; set; }
    }

    public class UpdateUserProfile
    {
        public string Name { get; set; }
        public string Preferences { get; set; }
    }
}
