using System;
using System.Collections.Generic;

namespace mosPortal.Models
{
    public partial class Concern
    {
        public Concern()
        {
            Comment = new HashSet<Comment>();
            UserConcern = new HashSet<UserConcern>();
        }

        public int Id { get; set; }
        public string Text { get; set; }
        public int UserId { get; set; }
        public DateTime? Date { get; set; }
        public string Title { get; set; }
        public int CategoryId { get; set; }

        public Category Categorie { get; set; }
        public User User { get; set; }
        public ICollection<Comment> Comment { get; set; }
        public ICollection<UserConcern> UserConcern { get; set; }
    }
}
