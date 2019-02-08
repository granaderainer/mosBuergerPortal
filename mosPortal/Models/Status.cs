using System.Collections.Generic;
using Newtonsoft.Json;

namespace mosPortal.Models
{
    public class Status
    {
        public Status()
        {
            Concern = new HashSet<Concern>();
        }

        public int Id { get; set; }
        public string Description { get; set; }

        [JsonIgnore] public ICollection<Concern> Concern { get; set; }

        public ICollection<Poll> Poll { get; set; }
    }
}