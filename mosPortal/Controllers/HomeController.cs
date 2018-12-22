using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mosPortal.Models;

namespace mosPortal.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ConcernsView()
        {
            //@model IEnumerable<mosPortal.Models.Concern>
            IEnumerable<Concern> concerns = new List<Concern>();
            concerns.Append(new Concern {
                Id = 1,
                Text = "Hallo Das ist ein Test",
                UserId = 1
            });

            return View(concerns);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
