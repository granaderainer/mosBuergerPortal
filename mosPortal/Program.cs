using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using mosPortal.Data;
using Microsoft.EntityFrameworkCore;
using mosPortal.Models;
using Microsoft.AspNetCore.Identity;

namespace mosPortal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();

            // Testing Classes:
            

            

           /* dbbuergerContext db = new dbbuergerContext();
            var concern = db.Concern.Where(c => c.Id == 1);

            /*Address address = new Address
                { Country = "DE",
            City = "Eberstadt",
            ZipCode =  74189,
            Number = "4",
            Street  = "Weinsberger Straße"
            };
            db.Add(new User
            {
                Name = "Jens",
                Firstname = "Olaf",
                Email = "Jensolaf",
                UserName = "gustav",
                Birthday = "okj",
                Birthplace = "Öhringen",
                AddressId = 1,
                Password = "geheim1!",
                Address = address
                
            });
            db.SaveChanges();
            /*
            using (var context = new dbbuergerContext())
            {
                var address = new Address("Deutschland", "Weinsberg", 74189, "KWBStr", "4");

                context.Address.Add(address);
                context.SaveChanges();
            }
            
            using (var context = new dbbuergerContext())
            {
                var addresses = context.Address;
                foreach (var address in addresses)
                {
                    Console.WriteLine(address.Street);
                }
            }*/
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
