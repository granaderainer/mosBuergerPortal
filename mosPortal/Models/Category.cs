using System;
using System.Collections.Generic;

namespace mosPortal.Models
{
    public partial class Category
    {
        public Category()
        {
            Concern = new HashSet<Concern>();
        }

        public int Id { get; set; }
        public string Description { get; set; }

        public ICollection<Concern> Concern { get; set; }
        public ICollection<Poll> Poll { get; set; }
    }
}
