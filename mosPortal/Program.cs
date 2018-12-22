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

namespace mosPortal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var context = new dbbuergerContext())
            {
                var address = new Address("Deutschland", "Weinsberg", 74189, "KWBStr", "4");

                context.Address.Add(address);
                context.SaveChanges();
            }
            //CreateWebHostBuilder(args).Build().Run();
            using (var context = new dbbuergerContext())
            {
                var addresses = context.Address;
                foreach (var address in addresses)
                {
                    Console.WriteLine(address.Street);
                }
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
