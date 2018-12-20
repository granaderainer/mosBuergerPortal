using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore;

namespace mosPortal.Models
{
    public class Address : DbContext
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
        public Address(DbContextOptions<Address> options) : base (options)
        {

        }
    }
}
