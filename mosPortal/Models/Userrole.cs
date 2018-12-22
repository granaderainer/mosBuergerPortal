using System;
using System.Collections.Generic;

namespace mosPortal.Models
{
    public partial class Userrole
    {
        public Userrole()
        {
            User = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Description { get; set; }
        public string Role { get; set; }

        public ICollection<User> User { get; set; }
    }
}
