using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
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
using Microsoft.EntityFrameworkCore.ChangeTracking;
using File = mosPortal.Models.File;

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
            ViewData["VotesCount"] = db.UserConcern.Count();
            ViewData["VotedUserCount"] = db.UserConcern.GroupBy(uc => uc.UserId).Count();
            return View();
        }
        //bei Schow Concern Prüfen ob man nicht den einen Concern übergeben kann => keine Weitere DB Abfrage nötig
        [AllowAnonymous]
        public IActionResult ShowConcerns()
        {
          
            List<SelectListItem> categoriesList = new List<SelectListItem>();
            List<Category> categories = db.Category.ToList();

            ViewData["Categories"] = categories;
            foreach (Category category in categories)
            {
                categoriesList.Add(new SelectListItem { Value = category.Id.ToString(), Text = category.Description });
            }
            ViewData["CategoriesList"] = categoriesList;
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
            Concern concern = db.Concern.SingleOrDefault(c => c.Id == concernId);
            Category category = db.Category.SingleOrDefault(c => c.Id == concern.CategoryId);
            List<Image> images = db.Image.Where(i => i.ConcernId == concernId).Select(ii => new Image
            {
                Id = ii.Id
            }).ToList();
            List<Comment> comments = db.Comment.Where(c => c.ConcernId == concernId).ToList();
            List<UserConcern> userConcerns = db.UserConcern.Where(uc => uc.ConcernId == concern.Id).ToList();
            
            concern.UserConcern = userConcerns;
            concern.Comment = comments;
            concern.Category = category;
            concern.Image = images;
            
            return View("ConcernView", concern);
        }

        public JsonResult GetConcernJson(int concernId)
        {
            Concern concern = db.Concern.Where(c => c.Id == concernId).SingleOrDefault();
            List<File> files = db.File.Where(f => f.ConcernId == concernId).ToList();
            List<Image> images = db.Image.Where(i => i.ConcernId == concernId).ToList();

            int[] imageIds = new int[images.Count()];
            int[] fileIds = new int[files.Count()];
            int k = 0;
            int j = 0;
            foreach (File file in files)
            {
                fileIds[k] = file.Id;
                k++;
            }

            foreach (Image image in images)
            {
                imageIds[j] = image.Id;
                j++;
            }

            int categoryId = concern.CategoryId;
            return Json(new
            {
                concernId, title = concern.Title, text = concern.Text, categoryId, date = concern.Date.ToString(),
                imageIds, fileIds
            });
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
        [HttpPost()]
        public async Task<IActionResult> CreateConcernAsync(Concern concern, List<IFormFile> files)
        {

            if (ModelState.IsValid)
            {
               // Image[] files = concern.Image.ToArray();
                concern.UserId = (await userManager.GetUserAsync(HttpContext.User)).Id;
                concern.Date = DateTime.UtcNow;

                long size = files.Sum(f => f.Length);

                // full path to file in temp location
                var filePath = Path.GetTempFileName();
                foreach (var formFile in files)
                {
                    mosPortal.Models.File file = new mosPortal.Models.File();
                    Image image = new Image();
                    if (formFile.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await formFile.CopyToAsync(stream);

                            if (string.Equals(formFile.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase)) /*"\"application/pdf\"")*/
                            {
                                file.File1 = stream.ToArray();
                                file.ConcernId = concern.Id;
                                file.PollId = null;
                                file.Name = formFile.FileName;
                                file.Ending = formFile.ContentType;
                                concern.File.Add(file);
                            }
                            else
                            {
                                image.Img = stream.ToArray();
                                image.ConcernId = concern.Id;
                                image.PollId = null;
                                image.Name = formFile.FileName;
                                image.Ending = formFile.ContentType;
                                concern.Image.Add(image);
                            }
                            
                            
                            
                        }
                        
                    }
                    
                }


                concern.StatusId = 1;
                db.Add(concern);
                await db.SaveChangesAsync();
                return RedirectToAction("ShowConcern", "Home", new { concernId = concern.Id });
            }
            else
            {
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

        
        public IActionResult ShowPolls()
        {
            //int pollId = -1;
            DateTime time = DateTime.UtcNow;

            List<Poll> polls = db.Poll.Where(p => p.End>time).Where(p=> p.NeedsLocalCouncil == false).Where(p=> p.Approved == true).Include("Category").ToList();

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
            ICollection<PollViewModel> pollViewModels = new List<PollViewModel>();
            foreach (Poll poll in polls)
            {
                
                PollViewModel pollViewModel = new PollViewModel();

                pollViewModel.Id = poll.Id;
                pollViewModel.Text = poll.Text;
                if (pollViewModel.End == null)
                {
                   pollViewModel.End = DateTime.UtcNow;
                    }
                else
                {
                   pollViewModel.End = (DateTime) poll.End;
                  }
                
                pollViewModel.UserId = poll.UserId;
                pollViewModel.NeedsLocalCouncil = poll.NeedsLocalCouncil;
                pollViewModel.Approved = poll.Approved;
                pollViewModel.CategoryId = poll.CategoryId;
                pollViewModel.Title = poll.Title;
                pollViewModel.Category = poll.Category;
                pollViewModel.User = poll.User;
                pollViewModel.AnswerOptionsPoll = poll.AnswerOptionsPoll;
                pollViewModel.RadioId = 0;
                

                pollViewModels.Add(pollViewModel);

            }

            return View("PollsView",pollViewModels);

        }

        [Authorize(Policy = "AllRoles")]
        [HttpPost]
        public async Task<IActionResult> submitPollAnswer(PollViewModel poll)
        {
            if (ModelState.IsValid)
            {
                var selectedRadio = poll.RadioId;
                var pollId = poll.Id;
                var user = await userManager.GetUserAsync(HttpContext.User);


                var matchEntry = db.AnswerOptionsPoll.Where(aop => aop.PollId == pollId)
                    .Where(aop => aop.AnswerOptionsId == selectedRadio);

                var selectedAnswerOptionsPollId = -1;
                selectedAnswerOptionsPollId = matchEntry.First().Id;
                //Neues aktuelles Abstimmungsobjekt
                var userAnswerOptionsPoll = new UserAnswerOptionsPoll();
                userAnswerOptionsPoll.AnswerOptionsPollId = selectedAnswerOptionsPollId;
                userAnswerOptionsPoll.UserId = user.Id;

                //den User in der AnsweroptionsPoll suchen
                var anyAnswers = db.AnswerOptionsPoll.FromSql(
                    "select * from User_AnswerOptions_Poll as uaop  LEFT JOIN AnswerOptions_Poll AS aop on uaop.answerOptions_poll_ID = aop.ID where aop.poll_ID = {0} and uaop.User_ID={1};",
                    pollId,user.Id).ToList();

                //Gibt es schon eine Abstimmung des Users (ja count != 0)
                if (anyAnswers.Count != 0)
                {
                    //db entry wird gelöscht und ein neuer angelegt, mit der neuen ID
                    var userAnswerOptionsPolltmp = new UserAnswerOptionsPoll();
                    userAnswerOptionsPolltmp.AnswerOptionsPollId = anyAnswers.First().Id;
                    userAnswerOptionsPolltmp.UserId = user.Id;
                    db.UserAnswerOptionsPoll.Remove(userAnswerOptionsPolltmp);
                    db.SaveChanges();
                }

                db.UserAnswerOptionsPoll.Add(userAnswerOptionsPoll);
                db.SaveChanges();
            }

            return ShowPolls();
        }


        public IActionResult ShowImpressum()
        {
            throw new NotImplementedException();
        }
    }
}
