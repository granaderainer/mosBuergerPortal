using System;
using System.Collections.Generic;

namespace mosPortal.Models
{
    public partial class UserConcern
    {
        public int UserId { get; set; }
        public int ConcernId { get; set; }

        public Concern Concern { get; set; }
        public User User { get; set; }
    }
}
