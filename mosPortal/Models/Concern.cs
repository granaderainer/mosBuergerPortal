using System;
using System.Collections.Generic;

namespace mosPortal.Models
{
    public partial class Concern
    {
        public Concern()
        {
            UserConcern = new HashSet<UserConcern>();
        }

        public int Id { get; set; }
        public string Text { get; set; }
        public int UserId { get; set; }

        public User User { get; set; }
        public ICollection<UserConcern> UserConcern { get; set; }
    }
}
