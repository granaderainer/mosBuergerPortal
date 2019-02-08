using Newtonsoft.Json;

namespace mosPortal.Models
{
    public class UserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int Id { get; set; }

        public Role Role { get; set; }

        [JsonIgnore] public User User { get; set; }
    }
}