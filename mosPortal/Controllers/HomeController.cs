using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mosPortal.Models;
using mosPortal.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace mosPortal.Controllers
{
    public class HomeController : Controller
    {
        private dbbuergerContext db = new dbbuergerContext();
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ConcernsView()
        {
            ViewData["Categories"] = db.Category;
            var concerns = db.Concern
                            .Include("Category")
                            .Where(x=>x.CategoryId == x.Category.Id)
                            .Select (x => new Concern
                                      {
                                          Id =x.Id,
                                          Text= x.Text,
                                          Title = x.Title,
                                          Date = x.Date,
                                          Category = x.Category,
                                          UserId= x.UserId
                                      });
            

            return View(concerns);
        }

        public JsonResult VoteForConcern(int concernId, int userId)
        {
            User user = db.User.Where(u => u.Id == userId).SingleOrDefault();
            ICollection < UserConcern > userConcerns = (ICollection<UserConcern>)db.UserConcern.Where(uc => uc.UserId == user.Id);
            user.UserConcern = userConcerns;
            if (user.allowToVote(concernId))
            {
            Concern concern = db.Concern
                .Where(c => c.Id == concernId).SingleOrDefault();
            userConcerns = (ICollection<UserConcern>)db.UserConcern.Where(uc => uc.ConcernId == concernId);
            concern.UserConcern = userConcerns;
                return Json(new { id = concern.Id, user = userId, votes = concern.vote(userId) });

            }
            return Json(new { });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
