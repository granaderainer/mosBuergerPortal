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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using mosPortal.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace mosPortal.Controllers
{
    public class HomeController : Controller
    {
        private dbbuergerContext db = new dbbuergerContext();
        private SignInManager<User> signInManager;
        private UserManager<User> userManager;


        public HomeController(UserManager<User> userManager, SignInManager<User> signManager)
        {
            this.userManager = userManager;
            this.signInManager = signManager;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult ShowConcerns()
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
            

            return View("ConcernsView",concerns);
        }
        [Authorize(Roles = "User,Administrator")]
        public IActionResult ShowConcern(int concernId)
        {
            Concern concern = db.Concern.Where(c => c.Id == concernId).SingleOrDefault();
            List<Comment> comments = db.Comment.Where(c => c.ConcernId == concernId).ToList();
            concern.Comment = comments;
            return View("ConcernView", concern);
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
        
        [HttpPost]
        public async Task<IActionResult> PostCommentAsync(int concernId, string commentText)
        {
            //string commentText = model.Text;
            DateTime time = DateTime.UtcNow;
            var user = await userManager.GetUserAsync(HttpContext.User);
            Comment comment = new Comment
            {
                Text = commentText,
                Date = time,
                ConcernId = concernId,
                UserId = user.Id
            };
            db.Comment.Add(comment);
            db.SaveChanges();
            return this.ShowConcern(concernId);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
