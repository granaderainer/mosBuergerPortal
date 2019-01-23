using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mosPortal.Data;
using mosPortal.Models;
using mosPortal.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mosPortal.Controllers
{
    public class AdministrationController : Controller
    {
        private dbbuergerContext db = new dbbuergerContext();
        private SignInManager<User> signInManager;
        private UserManager<User> userManager;

        public AdministrationController(UserManager<User> userManager, SignInManager<User> signManager)
        {
            this.userManager = userManager;
            this.signInManager = signManager;
        }
        public IActionResult Index()
        {
            ViewData["ConcernStatusOneCount"] = db.Concern.Where(c=> c.StatusId == 1).Count();
            ViewData["ConcernStatusTwoCount"] = db.Concern.Where(c => c.StatusId == 2).Include("UserConcern").Where(c=> c.UserConcern.Count >=1).Count();
            ViewData["ConcernStatusThreeCount"] = db.Concern.Where(c => c.StatusId == 3).Count();
            //Ablauf Datum!!!
            ViewData["PollCount"] = db.Poll.Count();
            ViewData["CurrentPolls"] = db.Poll.Where(p => p.End >= DateTime.UtcNow).OrderBy(p=> p.End).Take(4).ToList();
            return View("Index");
        }
        public async Task<IActionResult> ShowConcerns(int statusId = 0)
        {
            bool allowToCreateConcern = false;
            List<SelectListItem> statusList = new List<SelectListItem>();
            var user = await userManager.GetUserAsync(HttpContext.User);
            IList<string> roles = await userManager.GetRolesAsync(user);
            List<Concern> concerns = new List<Concern>();
            List<Status> status = new List<Status>();
            statusList.Add(new SelectListItem { Value = "0", Text = "Alle Anliegen" });
            if (roles[0]== "Verw")
            {
                concerns = db.Concern.Where(c => c.StatusId != 4 && !(c.StatusId >= 6)).Include("LastUpdatedByUser").ToList();
                status = db.Status.Where(s => s.Id != 4 && s.Id <= 5 ).ToList();
                allowToCreateConcern = true;
            }
            if(roles[0] == "GR")
            {
                concerns = db.Concern.Where(c => c.StatusId != 1 && c.StatusId != 3 && !(c.StatusId >= 5)).Include("LastUpdatedByUser").ToList();
                status = db.Status.Where(s => s.Id != 1 && s.Id != 3 && !(s.Id >= 5)).ToList();
            }
            if(roles[0] == "Admin")
            {
                concerns = db.Concern.Include("LastUpdatedByUser").ToList();
                status = db.Status.ToList();
            }
            foreach (Concern concern in concerns)
            {
                int length = 100;
                string text = concern.Text;
                if(text.Length >length)
                {
                    text = text.Substring(0, length)+"...";
                }
                concern.Text = text;
                concern.Comment = db.Comment.Where(c => c.ConcernId == concern.Id).ToList();
                concern.UserConcern = db.UserConcern.Where(uc => uc.ConcernId == concern.Id).ToList();
            }
            foreach (Status stat in status)
            {
                statusList.Add(new SelectListItem { Value = stat.Id.ToString(), Text = stat.Description });
                if(stat.Id == 3){
                    statusList.Add(new SelectListItem { Value = "31", Text = stat.Description + " von mir" });
                }
            }
            
            List<SelectListItem> categoriesList = new List<SelectListItem>();
            List<Category> categories = db.Category.ToList();
            categoriesList.Add(new SelectListItem { Value = "0", Text = "Alle Kategorien" });
            foreach (Category category in categories)
            {
                categoriesList.Add(new SelectListItem { Value = category.Id.ToString(), Text = category.Description });
            }
            //ViewData Variablen
            ViewData["allowToCreateConcern"] = allowToCreateConcern;
            ViewData["statusList"] = statusList;
            ViewData["CategoriesList"] = categoriesList;
            return View("ConcernsAdministrationView", concerns);
            /*if (statusId == 0)
            {
                concerns = db.Concern.ToList();
                foreach (Concern concern in concerns)
                {
                    concern.Comment = db.Comment.Where(c => c.ConcernId == concern.Id).ToList();
                    concern.UserConcern = db.UserConcern.Where(uc => uc.ConcernId == concern.Id).ToList();
                }
                return View("ConcernsAdministrationView", concerns);
            }
            
            //ViewData["Status"] = db.Status.Where(s => s.Id == statusId).SingleOrDefault().Description;
            if (statusId != 2)
            {
                concerns = db.Concern
                    .Where(c => c.StatusId == statusId)
                    .Include("Category")
                    .Where(c => c.CategoryId == c.Category.Id)
                    .Include("Status")
                    .Where(c => c.StatusId == c.Status.Id)
                    .Select(x => new Concern
                    {
                        Id = x.Id,
                        Text = x.Text,
                        Title = x.Title,
                        Date = x.Date,
                        StatusId = x.StatusId,
                        Category = x.Category,
                        UserId = x.UserId,
                        Status = x.Status
                    }).ToList();
                foreach (Concern concern in concerns)
                {
                    List<UserConcern> userConcerns = db.UserConcern.Where(uc => uc.ConcernId == concern.Id).ToList();
                    concern.UserConcern = userConcerns;
                }
                return View("ConcernsAdministrationView", concerns);
            }
            else
            {
                concerns = db.Concern.Where(c => c.StatusId == statusId).Include("UserConcern").Where(c => c.UserConcern.Count >= 1).ToList();
                return View("ConcernsAdministrationView", concerns);
            }*/
        }

        
        [HttpPost]
        public async Task<IActionResult> CreatePollAsync([FromBody]CreatePollModel createPoll)
        {

            if(ModelState.IsValid)
            {
                
                Concern concern = db.Concern.Where(c => c.Id == createPoll.ConcernId).SingleOrDefault();
                int concernStatusId = 6; //concern.StatusId;
                concern.StatusId = concernStatusId;
                db.Update(concern);
                int userId = (await userManager.GetUserAsync(HttpContext.User)).Id;
                List<int> answerOptionId = new List<int>();
                Poll poll = new Poll
                {
                    Text = createPoll.Text,
                    Title = createPoll.Title,
                    End = createPoll.End,
                    NeedsLocalCouncil = createPoll.NeedsLocalCouncil,
                    LastUpdatedBy = userId,
                    LastUpdatedAt = DateTime.UtcNow,
                    UserId = userId,
                    StatusId = 2,
                    CategoryId = createPoll.CategoryId
                };
                foreach (string answer in createPoll.Answers)
                {
                    AnswerOptions answerOption = null;
                    answerOption = db.AnswerOptions.Where(ao => ao.Description.Equals(answer)).SingleOrDefault();
                    if(answerOption != null)
                    {
                        answerOptionId.Add(answerOption.Id);
                    }
                    else
                    {
                        answerOption = new AnswerOptions { Description = answer };
                        db.Add(answerOption);
                        //db.SaveChanges();
                        answerOptionId.Add(answerOption.Id);
                    }
                }
                if(!poll.NeedsLocalCouncil)
                {
                    poll.Approved = true;
                }
                else
                {
                    poll.Approved = false;
                }
                db.Add(poll);
                foreach(int aoId in answerOptionId)
                {
                    db.Add(new AnswerOptionsPoll { PollId = poll.Id, AnswerOptionsId = aoId });
                }
                int result = db.SaveChanges();
                return Json(new { result , concernStatusId, concernId = createPoll.ConcernId});
            }
            else
            {
                return Json(new { result = 0 });
            }
            
        }
        [HttpGet]
        public async Task<IActionResult> GetConcernJson(int concernId)
        {
            // Variablen
            Concern concern = db.Concern.Where(c => c.Id == concernId).SingleOrDefault();
            Status[] statuses = null;
            List<File> files = db.File.Where(f => f.ConcernId == concernId).ToList();
            List<Image> images = db.Image.Where(i => i.ConcernId == concernId).ToList();
            var user = await userManager.GetUserAsync(HttpContext.User);
            IList<string> roles = await userManager.GetRolesAsync(user);
            int categoryId = concern.CategoryId;
            string comment = concern.AdminComment;
            // Auswahl der möglichen Status, die im Anliegen gesetzt werden können
            switch (concern.StatusId)
            {
                case 1:
                    statuses = db.Status.Where(s => s.Id <= (concern.StatusId +1)|| s.Id == 7).ToArray();
                    break;
                case 2:
                    statuses = db.Status.Where(s => s.Id >= concern.StatusId && s.Id <= 4).ToArray();
                    break;
                case 3:
                    statuses = db.Status.Where(s => s.Id >= concern.StatusId && s.Id <= 5).ToArray();
                    break;
                case 4:
                    statuses = db.Status.Where(s => s.Id >= concern.StatusId && s.Id <= 5).ToArray();
                    break;
            }
            //Admin kann alle Status zuordnen
            //Möglichkeit der Reaktivierung von Anliegen
            if(roles[0]== "Admin")
            {
                statuses = db.Status.ToArray();
            }
            //Datei und Bilder ID's für Anzeige extrahieren
            int[] imageIds = new int[images.Count()];
            int[] fileIds = new int[files.Count()];
            int k = 0;
            int j = 0;
            foreach(File file in files)
            {
                fileIds[k] = file.Id;
                    k++;
            }
            foreach(Image image in images)
            {
                imageIds[j] = image.Id;
                j++;
            }
            return Json(new { concernId, title = concern.Title, text= concern.Text,categoryId,statusId = concern.StatusId ,date = concern.Date.ToString(), statuses, imageIds, fileIds, comment });
        }
        /*[HttpPost]
        public async Task<IActionResult> ChangeConcernStatus(string concernModalStatus, string concernModalId)
        {
            int concernId = Convert.ToInt32(concernModalId);
            int statusId = Convert.ToInt32(concernModalStatus);
            Concern concern = db.Concern.Where(c => c.Id == concernId).SingleOrDefault();
            int oldStatus = concern.StatusId;
            concern.StatusId = statusId;
            db.Concern.Update(concern);
            db.SaveChanges();
            return await this.ShowConcerns(oldStatus);

        }*/
        [HttpPost]
        public async Task<JsonResult> ChangeConcernStatus(string status, string concern, string comment)
        {
            int userId = (await userManager.GetUserAsync(HttpContext.User)).Id;
            int concernId = Convert.ToInt32(concern);
            int statusId = Convert.ToInt32(status);
            Concern con = db.Concern.Where(c => c.Id == concernId).SingleOrDefault();
            int oldStatus = con.StatusId;
            con.StatusId = statusId;
            if (statusId == 3)
            {
                con.LastUpdatedBy = userId;
                con.LastUpdatedAt = DateTime.Now;
            }
            con.AdminComment = comment;
            db.Concern.Update(con);
            int result = db.SaveChanges();
            return Json(new { result });
        }
        public async Task<IActionResult> ShowPolls()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);
            IList<string> roles = await userManager.GetRolesAsync(user);
            List<Poll> polls = db.Poll.ToList();
            List<SelectListItem> categoriesList = new List<SelectListItem>();
            //List<SelectListItem> answerOptionList = new List<SelectListItem>();
            List<SelectListItem> statusList = new List<SelectListItem>();
            List<Category> categories = db.Category.ToList();
            List<AnswerOptions> answerOptions = db.AnswerOptions.ToList();
            statusList.Add(new SelectListItem { Value = "0", Text = "Alle Umfragen" });
            statusList.Add(new SelectListItem { Value = "2", Text = "laufende Umfragen" });
            statusList.Add(new SelectListItem { Value = "3", Text = "beendete Umfragen" });
            categoriesList.Add(new SelectListItem { Value = "0", Text = "Alle Kategorien" });
            if (roles[0] == "Verwaltung")
            {
                
            }
            if (roles[0] == "Gemeinderat")
            {
                
            }
            if (roles[0] == "Admin")
            {
                statusList.Add(new SelectListItem { Value = "5", Text = "abgeschlossene Umfragen" });
            }
            foreach (Category category in categories)
            {
                categoriesList.Add(new SelectListItem { Value = category.Id.ToString(), Text = category.Description });
            }
            /*foreach (AnswerOptions anserOption in answerOptions)
            {
                answerOptionList.Add(new SelectListItem { Value = anserOption.Id.ToString(), Text = anserOption.Description });
            }*/
            ViewData["CategoriesList"] = categoriesList;
            ViewData["StatusList"] = statusList;
            //ViewData["AnswerOptionsList"] = answerOptionList;
            /*foreach(Poll poll in polls)
            {
                List<AnswerOptionsPoll> answerOptionsPolls = db.AnswerOptionsPoll.Where(aop => aop.PollId == poll.Id).Include("AnswerOptions").Where(aop => aop.AnswerOptionsId == aop.AnswerOptions.Id).ToList();
                poll.AnswerOptionsPoll = answerOptionsPolls;
            }*/

            return View("PollsAdministrationView", polls );
        }
        [HttpGet]
        public IActionResult GetPollJson(int pollId)
        {
            int votes = 0;
            Poll poll = db.Poll.Where(p => p.Id == pollId).SingleOrDefault();
            List<AnswerOptionsPoll> answerOptionsPolls = db.AnswerOptionsPoll.Where(aop => aop.PollId == pollId).ToList();
            foreach(AnswerOptionsPoll answerOptionsPoll in answerOptionsPolls)
            {
                votes += db.UserAnswerOptionsPoll.Where(uaop => uaop.AnswerOptionsPollId == answerOptionsPoll.Id).Count();
            }
            return Json(new { title = poll.Title, text = poll.Text, end = poll.End, votes});

        }

        /*public IActionResult ShowPoll()
        {
            int id = 2;
            Poll poll = db.Poll.Where(p => p.Id == id).SingleOrDefault();
            List<AnswerOptionsPoll> answerOptionsPolls = db.AnswerOptionsPoll.Where(aop => aop.PollId == poll.Id).Include("AnswerOptions").Where(aop => aop.AnswerOptionsId == aop.AnswerOptions.Id).ToList();
            poll.AnswerOptionsPoll = answerOptionsPolls;
            return View("PollAdministrationView", poll);
        }*/
        public JsonResult GetPollAnswers(int pollId)
        {
            int id = pollId;
            Poll poll = db.Poll.Where(p => p.Id == id).SingleOrDefault();
            List<AnswerOptionsPoll> answerOptionsPolls = db.AnswerOptionsPoll.Where(aop => aop.PollId == poll.Id).Include("AnswerOptions").Where(aop => aop.AnswerOptionsId == aop.AnswerOptions.Id).ToList();
            foreach (AnswerOptionsPoll answerOptionsPoll in answerOptionsPolls)
            {
                List<UserAnswerOptionsPoll> userAnswerOptionsPolls = db.UserAnswerOptionsPoll.Where(uaop => uaop.AnswerOptionsPollId == answerOptionsPoll.Id).ToList();
                answerOptionsPoll.UserAnswerOptionsPoll = userAnswerOptionsPolls;
            }
            poll.AnswerOptionsPoll = answerOptionsPolls;

            return poll.getAnswers();

        }
        public IActionResult ShowCategories()
        {
            List<Category> categories = db.Category.ToList();
            return View("CategoriesAdministrationView", categories);
        }
        [HttpPost]
        public IActionResult crudCategory(string categoryId, string description, string operation)
        {
            Category category = new Category {};
            string title = "";
            string text = "";
            int result = 0;
            int categoryIdInt = Convert.ToInt32(categoryId);
            try
            {
                if (Equals(operation, "update"))
                {
                    title = "Update erfolgreich";
                    text = "Kategorie '" + description + "' erfolgreich geändert!";
                    result = 2;
                    category = db.Category.Where(c => c.Id == categoryIdInt).SingleOrDefault();
                    category.Description = description;
                    db.Update(category);
                    db.SaveChanges();
                }
                if (Equals(operation, "create"))
                {
                    title = "Erstellung erfolgreich";
                    text = "Kategorie '" + description + "' erfolgreich angelegt!"; 
                    result = 1;
                    category.Description = description;
                    db.Add(category);
                    db.SaveChanges();
                    categoryIdInt = category.Id;
                }
                if (Equals(operation, "delete"))
                {
                    result = 3;
                }
            }
            catch(Exception e)
            {
                title = "Fehler";
                text = e.Message;
                result = 0;
            }
            return Json(new { result, title, text, description, categoryId = categoryIdInt });
        }
        public IActionResult ShowUsers()
        {
            List<User> users = db.User.Include("UserRole").ToList();
            List<SelectListItem> rolesList = new List<SelectListItem>();
            List<Role> roles = db.Role.ToList();
            foreach (Role role in roles)
            {
                bool disabled = false;
                string description = "";
                switch (role.Name)
                {
                    case "GR":
                        int count = db.UserRole.Where(ur => ur.RoleId == role.Id).Count();
                        if (count >= 35) disabled = true;
                        description = role.Description + "(" + count + "/35)";
                        break;
                    case "BM":
                        if (db.UserRole.Where(ur => ur.RoleId == role.Id).Count() >= 1) disabled = true;
                        description = role.Description;
                        break;
                    default:
                        description = role.Description;
                        break;

                }
                rolesList.Add(new SelectListItem { Value= role.Id.ToString(), Text=description,Disabled=disabled });
            }
            ViewData["Roles"] = rolesList;
            return View("UsersAdministrationView", users);
        }
        [HttpGet]
        public IActionResult GetUser(int userId)
        {
            //int userIdInt = Convert.ToInt32(userId);
            User user = db.User.Where(u => u.Id == userId).Include("Address").Include("UserRole").SingleOrDefault();
            user.Password = "";
            return Json(user);
        }
        [HttpPost]
        public IActionResult crudUser(User user, int roleId, string operation)
        {
            string title = "";
            string text = "";
            string localCouncilDescription = "";
            bool localCouncilFull = false;
            bool mayorFull = false;
            int localCouncilId = 4;
            int mayorId = 3;
            int result = 0;
            User dbUser = db.User.Where(u => u.Id == user.Id).SingleOrDefault();
            dbUser.Firstname = user.Firstname;
            dbUser.Name = user.Name;
            dbUser.Birthplace = user.Birthplace;
            dbUser.Birthday = user.Birthday;
            //dbUser.UserName = user.UserName;
            dbUser.Email = user.Email;
            db.Update(dbUser);
            try
            {
                UserRole userRole = db.UserRole.Where(ur => ur.UserId == user.Id).SingleOrDefault();
                userRole.RoleId = roleId;
                db.Update(userRole);
            }
            catch
            {
                UserRole userRole = new UserRole { UserId = user.Id, RoleId = roleId };
                db.Add(userRole);
            }
            result = db.SaveChanges();
            //GR zählen
            int count = db.UserRole.Where(ur => ur.RoleId == localCouncilId).Count();
            localCouncilDescription = db.Role.Where(r => r.Id == localCouncilId).SingleOrDefault().Description;
            localCouncilDescription = localCouncilDescription + "("+count+"/35)";
            if (count >= 35) localCouncilFull = true;
            //BM zählen
            if (db.UserRole.Where(ur => ur.RoleId == mayorId).Count() >= 1) mayorFull = true;
            return Json(new { result, user, roleId, title, text, localCouncilDescription,localCouncilFull, mayorFull });
        }

        /*[HttpGet]
        public IActionResult CreatePoll()
        {
            Poll poll = new Poll();
            List<SelectListItem> categoriesList = new List<SelectListItem>();
            List<SelectListItem> answerOptionList = new List<SelectListItem>();
            List<Category> categories = db.Category.ToList();
            List<AnswerOptions> answerOptions = db.AnswerOptions.ToList();
            foreach (Category category in categories)
            {
                categoriesList.Add(new SelectListItem { Value = category.Id.ToString(), Text = category.Description });
            }
            foreach(AnswerOptions anserOption in answerOptions)
            {
                answerOptionList.Add(new SelectListItem { Value = anserOption.Id.ToString(), Text = anserOption.Description });
            }
            ViewData["CategoriesList"] = categoriesList;
            ViewData["AnswerOptionsList"] = answerOptionList;
            return View("CreatePollAdministrationView", poll);
        }*/

        /*public IActionResult ShowConcernsLocalCouncil()
{
    List<Concern> concerns = db.Concern.Where(c => c.StatusId >= 2 && c.StatusId <= 3).Include("UserConcern").Where(c => c.UserConcern.Count >= 1).ToList();
    return View("ConcernsAdministrationView", concerns);
}*/
    }
}
