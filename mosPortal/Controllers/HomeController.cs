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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        //bei Schow Concern Prüfen ob man nicht den einen Concern übergeben kann => keine Weitere DB Abfrage nötig
        [AllowAnonymous]
        public IActionResult ShowConcerns()
        {
            ViewData["Categories"] = db.Category;
            List<Concern> concerns = db.Concern
                            .Where(c=> c.StatusId == 2 || c.StatusId == 3)
                            .Include("Category")
                            .Where(c=>c.CategoryId == c.Category.Id)
                            .Select (x => new Concern
                                      {
                                          Id =x.Id,
                                          Text= x.Text,
                                          Title = x.Title,
                                          Date = x.Date,
                                          Category = x.Category,
                                          UserId= x.UserId
                                      }).ToList();
            foreach (Concern concern in concerns)
            {
                List<UserConcern> userConcerns = db.UserConcern.Where(uc => uc.ConcernId == concern.Id).ToList();
                concern.UserConcern = userConcerns;
            }
            return View("ConcernsView",concerns);
        }
        [Authorize(Policy = "AllRoles")]
        public IActionResult ShowConcern(int concernId)
        {
            Concern concern = db.Concern.Where(c => c.Id == concernId).SingleOrDefault();
            List<Comment> comments = db.Comment.Where(c => c.ConcernId == concernId).ToList();
            List<UserConcern> userConcerns = db.UserConcern.Where(uc => uc.ConcernId == concern.Id).ToList();
            concern.UserConcern = userConcerns;
            concern.Comment = comments;
            return View("ConcernView", concern);
        }

        public async Task<JsonResult> VoteForConcernAsync(int concernId)
        {
            User user = await userManager.GetUserAsync(HttpContext.User);
            db.Add(new UserConcern
            {
                UserId = user.Id,
                ConcernId = concernId
            });
            await db.SaveChangesAsync();
            int votes = db.UserConcern.Where(uc => uc.ConcernId == concernId).Count();
            return Json(new { votes = votes});
        }

        [Authorize(Policy = "AllRoles")]
        [HttpGet]
        public IActionResult CreateConcern()
        {
                List<SelectListItem> categoriesList = new List<SelectListItem>();
                List<Category> categories = db.Category.ToList();
                foreach (Category category in categories)
                {
                    categoriesList.Add(new SelectListItem { Value = category.Id.ToString(), Text = category.Description });
                }
                ViewData["CategoriesList"] = categoriesList;
                return View("CreateConcernView");

        }
        [Authorize(Policy = "AllRoles")]
        [HttpPost]
        public async Task<IActionResult> CreateConcernAsync(Concern concern)
        {

            if (ModelState.IsValid)
            {
                concern.UserId = (await userManager.GetUserAsync(HttpContext.User)).Id;
                concern.Date = DateTime.UtcNow;
                concern.StatusId = 1;
                db.Add(concern);
                await db.SaveChangesAsync();
                return RedirectToAction("ShowConcern", "Home", new { concernId = concern.Id });
            }
            else {
                return View("CreateConcernView");
            }
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

        [Authorize(Policy = "AllRoles")]
        public IActionResult ShowPolls()
        {
            //IActionResult
            //DB Abfrage für Polls
            //Einstellen von Polls (Verwaltung)

            /*List<Poll> polls = db.Poll.Select(p => new Poll
            {
                Id = p.Id,
                Text = p.Text,
                End = p.End,
                UserId = p.UserId,
                NeedsLocalCouncil = p.NeedsLocalCouncil,
                Approved = p.Approved,
                AnswerOptionsPoll = db.AnswerOptionsPoll.Where(c => c.PollId == p.Id).ToList()


            }).ToList();*/
            int pollId = 1;
            //Where enddatum < aktuelles datum
            List<Poll> polls = db.Poll.ToList();
            foreach (Poll poll in polls)
            {
                List<AnswerOptionsPoll> answers = db.AnswerOptionsPoll.Where(aop => aop.PollId == poll.Id)
                    .Include("AnswerOptions")
                    .Where(aop => aop.AnswerOptionsId == aop.AnswerOptions.Id).Select(aop => new AnswerOptionsPoll
                        {
                            Id = aop.Id,
                            AnswerOptionsId = aop.AnswerOptionsId,
                            PollId = aop.PollId,
                            AnswerOptions = aop.AnswerOptions
                        }
                    ).ToList();
                poll.AnswerOptionsPoll = answers;
                
            }

        
        //AnswerOptions = db.AnswerOptions.Where(a => a.Id == c.Id)
            //List<AnswerOptionsPoll> answerOptionsPolls = db.AnswerOptionsPoll.Where(c => c.PollId == 0).ToList();
            return View("PollsView",polls);

        }

        //public Task<IActionResult> submitPollAnswer(int id, [Bind("ID,Title,ReleaseDate,Genre,Price")])
        [HttpPost]
        public Task<IActionResult> submitPollAnswer(Poll poll, string returnUrl =null, string radioValue = "1")
        {
            
            if (ModelState.IsValid)
            {
                
                //gehtcurrentUser for Poll
                //poll.User = (await userManager.GetUserAsync(HttpContext.User)).Id;
                //getcurrentDate for Database
                DateTime date = DateTime.UtcNow;
                //Anwort in DB (User_Concern)
                //await db.SaveChangesAsync();
                //return RedirectToAction("ShowConcern", "Home", new { concernId = concern.Id });


                //return RedirectToAction("ShowConcern", "Home", new { concernId = concern.Id });
            }
            else
            {
                //return View("CreateConcernView");
            }

            //return View("PollsView", polls);
            return null;
        }


    }
}
