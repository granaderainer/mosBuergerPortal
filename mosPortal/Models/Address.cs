using System.Collections.Generic;
using Newtonsoft.Json;

namespace mosPortal.Models
{
    public class Address
    {
        public Address()
        {
            User = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int? ZipCode { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }

        [JsonIgnore] public ICollection<User> User { get; set; }
    }
}