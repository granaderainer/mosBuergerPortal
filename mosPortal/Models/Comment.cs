using System;
using System.Collections.Generic;

namespace mosPortal.Models
{
    public partial class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime? Date { get; set; }
        public int UserId { get; set; }
        public int ConcernId { get; set; }

        public Concern Concern { get; set; }
        public User User { get; set; }
    }
}
