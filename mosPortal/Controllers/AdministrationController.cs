using Microsoft.AspNetCore.Mvc;
using mosPortal.Data;
using mosPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mosPortal.Controllers
{
    public class AdministrationController : Controller
    {
        private dbbuergerContext db = new dbbuergerContext();
        public IActionResult Index()
        {
            ViewData["ConcernCount"] = 10;//db.Concern.Count();
            ViewData["PollCount"] = 15;//db.Concern.Count();
            return View("Index");
        }
    }
}
