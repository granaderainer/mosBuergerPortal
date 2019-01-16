using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace mosPortal.Models
{
    public partial class UserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int Id { get; set; }

        public Role Role { get; set; }
        [JsonIgnore]
        public User User { get; set; }

    }
}
