using System;
using System.Collections.Generic;

namespace mosPortal.Models
{
    public partial class Address
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

        public ICollection<User> User { get; set; }
        public Address(string country, string city, int zipCode, string street, string number)
        {
            this.Country = country;
            this.City = city;
            this.ZipCode = zipCode;
            this.Street = street;
            this.Number = number;
            
        }
    }
}
