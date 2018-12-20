using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mosPortal.Models
{
    public class Address
    {
        private int id;
        private String country;
        private String city;
        private int zipCode;
        private String street;
        private int number;

        public Address (int id)
        {
            this.id = id;

            // DB Abfrage für User!
        }

        public Address(string country, string city, int zipCode, string street, int number)
        {
            this.country = country;
            this.city = city;
            this.zipCode = zipCode;
            this.street = street;
            this.number = number;
        }
    }
}
