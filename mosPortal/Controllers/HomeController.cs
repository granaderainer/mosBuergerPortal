using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using mosPortal.Data;
using mosPortal.Models;
using mosPortal.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using File = mosPortal.Models.File;

namespace mosPortal.Controllers
{
    //Jeder hat Zugriff auf den Kontroller (außer die Methoden sind abgesichert)
    //Kontroller für die Einwohneransicht
    public class HomeController : Controller
    {
        private readonly dbbuergerContext db;
        private SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

        //Konstrukor
        public HomeController(UserManager<User> userManager, SignInManager<User> signManager, dbbuergerContext db)
        {
            this.userManager = userManager;
            this.db = db;
            signInManager = signManager;
        }

        //Startseite Index
        [AllowAnonymous]
        public async Task<IActionResult> Index(bool register = false, bool login = false) //Startup Page
        {
            ViewData["register"] = register; 
            ViewData["login"] = login;
            ViewData["VotesCount"] = db.UserConcern.Count();
            ViewData["VotedUserCount"] = db.UserConcern.GroupBy(uc => uc.UserId).Count();
            ViewData["PollViewModels"] = await ShowPollsIndex();
            return View("Index"); //Call View "Index"
        }
        //Rolesystem
        [AllowAnonymous] 
        //Alle Anliegen anzeigen
        public IActionResult ShowConcerns()
        {
            DateTime time6 = DateTime.UtcNow.AddMonths(-6);
            List<SelectListItem> categoriesList = new List<SelectListItem>();
            List<Category> categories = db.Category.ToList(); //Database Query with Entity Framework

            ViewData["Categories"] = categories;
            foreach (Category category in categories)
            {
                categoriesList.Add(new SelectListItem {Value = category.Id.ToString(), Text = category.Description});
            }

            ViewData["CategoriesList"] = categoriesList;
            List<Concern> concerns = db.Concern
                .Where(c => c.StatusId == 2 || c.StatusId == 3)
                .Include("Category")
                .Include("Comment")
                .OrderBy(c => c.Date)
                .ToList();
            List<Concern> concernsRemoveList = new List<Concern>();
            foreach (Concern concern in concerns)
            {
                List<UserConcern> userConcerns = db.UserConcern.Where(uc => uc.ConcernId == concern.Id).ToList();
                concern.UserConcern = userConcerns;
                if (time6 > concern.Date && concern.StatusId == 2 && concern.UserConcern.Count < 100)
                {
                    concern.StatusId = 6;
                    db.Concern.Update(concern);
                    concernsRemoveList.Add(concern);
                }
            }

            db.SaveChangesAsync();
            foreach (Concern c in concernsRemoveList)
            {
                concerns.Remove(c);
            }

            return View("ConcernsView", concerns); //return View ConcernsView with an list of concerns
        }

        [Authorize(Policy = "AllRoles")] //Rolesystem, you have to be logged in, but the Role is not important
        //Ein Anliegen 
        public IActionResult ShowConcern(int concernId) //Only one concern, called by his id
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

            List<SelectListItem> categoriesList = new List<SelectListItem>();
            List<Category> categories = db.Category.ToList();

            ViewData["Categories"] = categories;
            foreach (Category c in categories)
            {
                categoriesList.Add(new SelectListItem {Value = c.Id.ToString(), Text = c.Description});
            }

            ViewData["CategoriesList"] = categoriesList;

            return View("ConcernView", concern);
        }
        
        //Anliegen Voten
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
            return Json(new {votes});
        }
        
        //Neues Anliegen erstellen
        [Authorize(Policy = "AllRoles")]
        [HttpPost]
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
                    File file = new File();
                    Image image = new Image();
                    if (formFile.Length > 0)
                    {
                        using (var stream = new MemoryStream())
                        {
                            await formFile.CopyToAsync(stream);

                            if (string.Equals(formFile.ContentType, "application/pdf",
                                StringComparison.OrdinalIgnoreCase)) /*"\"application/pdf\"")*/
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
                return RedirectToAction("ShowConcern", "Home", new {concernId = concern.Id});
            }

            return View("CreateConcernView");
        }
        //Kommentar bei einem Anliegen speichern
        [HttpPost]
        public async Task<IActionResult> PostCommentAsync(int concernId, string commentText) //post a comment async with javascript
        {
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
            return ShowConcern(concernId);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        //Alle Einwohner Umfragen ausgeben
        public async Task<IActionResult> ShowPolls() //show all Polls
        {
            DateTime time = DateTime.UtcNow;
            List<Poll> polls = db.Poll.Where(p => p.End > time).Where(p => p.NeedsLocalCouncil == false)
                .Where(p => p.Approved).Include("Category").OrderBy(p => p.End).ToList();
            List<SelectListItem> categoriesList = new List<SelectListItem>();
            List<Category> categories = db.Category.ToList();
            var user = await userManager.GetUserAsync(HttpContext.User);
            ViewData["Categories"] = categories;
            foreach (Category category in categories)
            {
                categoriesList.Add(new SelectListItem {Value = category.Id.ToString(), Text = category.Description});
            }

            ViewData["CategoriesList"] = categoriesList;
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
                List<Image> images = db.Image.Where(i => i.PollId == poll.Id).Select(ii => new Image
                {
                    Id = ii.Id
                }).ToList();
                List<File> files = db.File.Where(f => f.PollId == poll.Id).Select(ff => new File
                {
                    Id = ff.Id,
                    Name = ff.Name,
                    Ending = ff.Ending
                }).ToList();
                poll.AnswerOptionsPoll = answers;
                poll.Image = images;
                poll.File = files;
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
                pollViewModel.Image = poll.Image;
                pollViewModel.userAnswerOptionsPollId = 0;
                if (user != null)
                {
                    UserAnswerOptionsPoll userAnswerOptionsPoll = null;
                    foreach (AnswerOptionsPoll aop in poll.AnswerOptionsPoll)
                    {
                        userAnswerOptionsPoll = db.UserAnswerOptionsPoll
                            .Where(uaop => uaop.UserId == user.Id && uaop.AnswerOptionsPollId == aop.Id)
                            .SingleOrDefault();
                        if (userAnswerOptionsPoll != null)
                        {
                            break;
                        }
                    }

                    if (userAnswerOptionsPoll != null)
                    {
                        pollViewModel.userAnswerOptionsPollId = userAnswerOptionsPoll.AnswerOptionsPollId;
                    }
                }

                pollViewModels.Add(pollViewModel);
            }

            return View("PollsView", pollViewModels);
        }

        //Umfrage abstimmen   
        [Authorize(Policy = "AllRoles")]
        [HttpPost]
        public async Task<IActionResult> submitPollAnswer(PollViewModel poll) //submit Poll answer on the index page, and poll page
        {
            if (ModelState.IsValid)
            {
                int selectedRadio = poll.RadioId;
                int pollId = poll.Id;
                User user = await userManager.GetUserAsync(HttpContext.User);

                IQueryable<AnswerOptionsPoll> matchEntry = db.AnswerOptionsPoll.Where(aop => aop.Id == selectedRadio);
               
                int selectedAnswerOptionsPollId = -1;
                selectedAnswerOptionsPollId = matchEntry.First().Id;
               
                UserAnswerOptionsPoll userAnswerOptionsPoll = new UserAnswerOptionsPoll();
                userAnswerOptionsPoll.AnswerOptionsPollId = selectedAnswerOptionsPollId;
                userAnswerOptionsPoll.UserId = user.Id;

                List<AnswerOptionsPoll> anyAnswers = db.AnswerOptionsPoll.FromSql(
                    "select * from User_AnswerOptions_Poll as uaop  LEFT JOIN AnswerOptions_Poll AS aop on uaop.answerOptions_poll_ID = aop.ID where aop.poll_ID = {0} and uaop.User_ID={1};",
                    pollId, user.Id).ToList();

                if (anyAnswers.Count != 0)
                {
                    UserAnswerOptionsPoll userAnswerOptionsPolltmp = new UserAnswerOptionsPoll();
                    userAnswerOptionsPolltmp.AnswerOptionsPollId = anyAnswers.First().Id;
                    userAnswerOptionsPolltmp.UserId = user.Id;
                    db.UserAnswerOptionsPoll.Remove(userAnswerOptionsPolltmp);
                    db.SaveChanges();
                }

                db.UserAnswerOptionsPoll.Add(userAnswerOptionsPoll);
                db.SaveChanges();
            }
            
            return await ShowPolls();
        }


        public IActionResult ShowImpressum() //call Impressum view
        {
            return View("_Impressum");
        }

        public IActionResult ShowDSGVO()
        {
            return View("_Datenschutzerklaerung");
        }

        //Umfragen auf der Startseite anzeigen
        public async Task<ICollection<PollViewModel>> ShowPollsIndex()
        {
            DateTime time = DateTime.UtcNow;
            List<Poll> polls = db.Poll.Where(p => p.StatusId == 2).Where(p => p.End > time).Include("Category")
                .OrderBy(p => p.End).Take(4).ToList(); //.OrderBy(p => p.End)
            var user = await userManager.GetUserAsync(HttpContext.User);
           
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
                List<Image> images = db.Image.Where(i => i.PollId == poll.Id).Select(ii => new Image
                {
                    Id = ii.Id
                }).ToList();
                List<File> files = db.File.Where(f => f.PollId == poll.Id).Select(ff => new File
                {
                    Id = ff.Id,
                    Name = ff.Name,
                    Ending = ff.Ending
                }).ToList();
                poll.AnswerOptionsPoll = answers;
                poll.Image = images;
                poll.File = files;
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
                    pollViewModel.End = (DateTime)poll.End;
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
                pollViewModel.Image = poll.Image;
                pollViewModel.userAnswerOptionsPollId = 0;
                if (user != null)
                {
                    UserAnswerOptionsPoll userAnswerOptionsPoll = null;
                    foreach (AnswerOptionsPoll aop in poll.AnswerOptionsPoll)
                    {
                        userAnswerOptionsPoll = db.UserAnswerOptionsPoll
                            .Where(uaop => uaop.UserId == user.Id && uaop.AnswerOptionsPollId == aop.Id)
                            .SingleOrDefault();
                        if (userAnswerOptionsPoll != null)
                        {
                            break;
                        }
                    }

                    if (userAnswerOptionsPoll != null)
                    {
                        pollViewModel.userAnswerOptionsPollId = userAnswerOptionsPoll.AnswerOptionsPollId;
                    }
                }

                pollViewModels.Add(pollViewModel);
            }

            return pollViewModels;
        }

        //Umfrageergebnisse für den Einwohner anzeigen
        public IActionResult ShowPollResults() //Results of ended polls
        {
            DateTime time = DateTime.UtcNow;
            List<Poll> polls = db.Poll.Where(p => p.End < time).Where(p => p.NeedsLocalCouncil == false)
                .Where(p => p.Approved).Include("Category").ToList();
            
            List<SelectListItem> categoriesList = new List<SelectListItem>();
            List<Category> categories = db.Category.ToList();
           
            for (int i = 0; i < polls.Count; i++)
            {
                DateTime time3 = (DateTime) polls[i].End;
                time3 = time3.AddMonths(3);
                if (time > time3)
                {
                    polls.Remove(polls[i]);
                }
            }

            ViewData["Categories"] = categories;
            foreach (Category category in categories)
            {
                categoriesList.Add(new SelectListItem {Value = category.Id.ToString(), Text = category.Description});
            }

            ViewData["CategoriesList"] = categoriesList;
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
                List<Image> images = db.Image.Where(i => i.PollId == poll.Id).Select(ii => new Image
                {
                    Id = ii.Id
                }).ToList();
                List<File> files = db.File.Where(f => f.PollId == poll.Id).Select(ff => new File
                {
                    Id = ff.Id,
                    Name = ff.Name,
                    Ending = ff.Ending
                }).ToList();
                poll.AnswerOptionsPoll = answers;
                poll.Image = images;
                poll.File = files;
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
                pollViewModel.Image = poll.Image;

                pollViewModels.Add(pollViewModel);
            }

            return View("PollResultsView", pollViewModels);
        }
    }
}