using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mosPortal.Data;
using mosPortal.Models;
using mosPortal.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using File = mosPortal.Models.File;

namespace mosPortal.Controllers
{
    //Zugriff auf den gesamten Controller nur mit Administrativen Rollen: Admin, Verwaltung, Gemeinderat, Bürgermeister
    [Authorize(Policy = "AllAdministrationRoles")]
    public class AdministrationController : Controller
    {
        //Attribute der Klasse AdministrationController
        private readonly dbbuergerContext db;
        private SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly IHostingEnvironment _hostingEnvironment;

        //Konstruktor
        public AdministrationController(UserManager<User> userManager, SignInManager<User> signManager,
            IHostingEnvironment hostingEnvironment, dbbuergerContext db)
        {
            this.userManager = userManager;
            signInManager = signManager;
            _hostingEnvironment = hostingEnvironment;
            this.db = db;
        }

        //Index
        public IActionResult Index()
        {
            //ViewData Vaariablen mit den nötigen Informationen befüllen
            ViewData["ConcernStatusOneCount"] = db.Concern.Where(c => c.StatusId == 1).Count();
            ViewData["ConcernStatusTwoCount"] = db.Concern.Where(c => c.StatusId == 2).Include("UserConcern")
                .Where(c => c.UserConcern.Count >= 1).Count();
            ViewData["ConcernStatusThreeCount"] = db.Concern.Where(c => c.StatusId == 3).Count();
            //Ablauf Datum!!!
            //ViewData["PollCount"] = db.Poll.Count();
            ViewData["PollCount"] = db.Poll.Where(p => p.End > DateTime.UtcNow).Where(p => p.NeedsLocalCouncil == false)
                .Where(p => p.Approved).Count();
            ViewData["CurrentPolls"] =
                db.Poll.Where(p => p.End >= DateTime.UtcNow).OrderBy(p => p.End).Take(4).ToList();
            return View("Index");
        }

        //Alle Anliegen Anzeigen
        public async Task<IActionResult> ShowConcerns(int statusId = 0)
        {
            //Variablen
            bool allowToCreatePoll = false;
            List<SelectListItem> statusList = new List<SelectListItem>();
            var user = await userManager.GetUserAsync(HttpContext.User);
            IList<string> roles = await userManager.GetRolesAsync(user);
            List<Concern> concerns = new List<Concern>();
            List<Status> status = new List<Status>();
            statusList.Add(new SelectListItem { Value = "0", Text = "Alle Anliegen" });

            //Selektieren der Anliegen und der Status je nach Rolle des Benutzers
            if (roles[0] == "Verw")
            {
                concerns = db.Concern.Where(c => c.StatusId != 4 && !(c.StatusId >= 6)).Include("LastUpdatedByUser")
                    .ToList();
                status = db.Status.Where(s => s.Id != 4 && s.Id <= 5).ToList();
                allowToCreatePoll = true;
            }

            if (roles[0] == "GR")
            {
                concerns = db.Concern.Where(c => c.StatusId != 1 && c.StatusId != 3 && !(c.StatusId >= 5))
                    .Include("LastUpdatedByUser").ToList();
                status = db.Status.Where(s => s.Id != 1 && s.Id != 3 && !(s.Id >= 5)).ToList();
            }

            if (roles[0] == "Admin")
            {
                concerns = db.Concern.Include("LastUpdatedByUser").ToList();
                status = db.Status.ToList();
            }

            //kürzen der Texte für die Kompaktansicht
            foreach (Concern concern in concerns)
            {
                int length = 100;
                string text = concern.Text;
                if (text.Length > length)
                {
                    text = text.Substring(0, length) + "...";
                }

                concern.Text = text;
                concern.Comment = db.Comment.Where(c => c.ConcernId == concern.Id).ToList();
                concern.UserConcern = db.UserConcern.Where(uc => uc.ConcernId == concern.Id).ToList();
            }

            foreach (Status stat in status)
            {
                statusList.Add(new SelectListItem { Value = stat.Id.ToString(), Text = stat.Description });
                /*if (stat.Id == 3)
                {
                    statusList.Add(new SelectListItem {Value = "31", Text = stat.Description + " von mir"});
                }*/
            }
            List<SelectListItem> categoriesList = new List<SelectListItem>();
            List<Category> categories = db.Category.ToList();
            categoriesList.Add(new SelectListItem { Value = "0", Text = "Alle Kategorien" });
            foreach (Category category in categories)
            {
                categoriesList.Add(new SelectListItem { Value = category.Id.ToString(), Text = category.Description });
            }

            //ViewData Variablen
            ViewData["allowToCreatePoll"] = allowToCreatePoll;
            ViewData["statusList"] = statusList;
            ViewData["CategoriesList"] = categoriesList;
            return View("ConcernsAdministrationView", concerns);
        }
        //Erstellen einer Umfrage, Methode gibt JSON String zurück
        [HttpPost]
        public async Task<IActionResult> CreatePollAsync([FromBody] CreatePollModel createPoll)
        {
            if (ModelState.IsValid)
            {
                //Variablen
                int concernStatusId = 0;
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
                //Nur, wenn Umfrage aus Anliegen erstellt wird, wird das Anliegen auf Status "abgeschlossen" gesetzt.
                if (createPoll.ConcernId != 0)
                {
                    Concern concern = db.Concern.Where(c => c.Id == createPoll.ConcernId).SingleOrDefault();
                    concernStatusId = 6; //concern.StatusId;
                    concern.StatusId = concernStatusId;
                    db.Update(concern);
                }

                //Antworten aus Objekt auslesen und in Datenbank speichern
                foreach (string answer in createPoll.Answers)
                {
                    AnswerOptions answerOption = null;
                    //Prüfung, ob Antwort schon einmal verwendet wurde
                    answerOption = db.AnswerOptions.Where(ao => ao.Description.Equals(answer)).SingleOrDefault();
                    if (answerOption != null)
                    {
                        answerOptionId.Add(answerOption.Id);
                    }
                    else
                    {
                        answerOption = new AnswerOptions { Description = answer };
                        db.Add(answerOption);
                        answerOptionId.Add(answerOption.Id);
                    }
                }

                if (!poll.NeedsLocalCouncil)
                {
                    poll.Approved = true;
                }
                else
                {
                    poll.Approved = false;
                }
                //Umfrage Speichern
                db.Add(poll);

                //Antworten Speichern 
                foreach (int aoId in answerOptionId)
                {
                    db.Add(new AnswerOptionsPoll { PollId = poll.Id, AnswerOptionsId = aoId });
                }

                //Dokumente aus dem Anliegen in die Umfrage übernehmen
                if (createPoll.FileIds != null)
                {
                    foreach (int fileId in createPoll.FileIds)
                    {
                        File file = db.File.Where(f => f.Id == fileId).SingleOrDefault();
                        file.PollId = poll.Id;
                        db.Update(file);
                    }
                }
                //Bilder aus dem Anliegen in die Umfrage übernehmen
                if (createPoll.ImageIds != null)
                {
                    foreach (int imageId in createPoll.ImageIds)
                    {
                        Image image = db.Image.Where(i => i.Id == imageId).SingleOrDefault();
                        image.PollId = poll.Id;
                        db.Update(image);
                    }
                }
                //Commit
                int result = db.SaveChanges();
                return Json(new { result, concernStatusId, concernId = createPoll.ConcernId });
            }

            return Json(new { result = 0 });
        }
        //Gibt alle Daten eines Anliegen als JSON String zurück
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
            //Admin kann alle Status zuordnen
            //Möglichkeit der Reaktivierung von Anliegen
            if (roles[0] == "Admin")
            {
                statuses = db.Status.ToArray();
            }
            else
            {
                switch (concern.StatusId)
                {
                    case 1:
                        statuses = db.Status.Where(s => s.Id <= concern.StatusId + 1 || s.Id == 7).ToArray();
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
                    case 5:
                        statuses = db.Status.Where(s => s.Id >= concern.StatusId && s.Id <= 6).ToArray();
                        break;
                }
            }

            //Datei und Bilder ID's für Anzeige extrahieren
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

            return Json(new
            {
                concernId,
                title = concern.Title,
                text = concern.Text,
                categoryId,
                statusId = concern.StatusId,
                date = concern.Date.ToString(),
                statuses,
                imageIds,
                fileIds,
                comment
            });
        }
        //Status eines Anliegen ändern und speichern
        [HttpPost]
        public async Task<IActionResult> ChangeConcernStatus(string status, string concern, string comment)
        {
            //Variablen
            int userId = (await userManager.GetUserAsync(HttpContext.User)).Id;
            int concernId = Convert.ToInt32(concern);
            int statusId = Convert.ToInt32(status);
            //Anliegen aus DB lesen
            Concern con = db.Concern.Where(c => c.Id == concernId).SingleOrDefault();
            int oldStatus = con.StatusId;
            con.StatusId = statusId;
            //Falls neuer Status 3, Id des Users hinzufügen
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
        //Zeigt alle Umfragen an
        public async Task<IActionResult> ShowPolls()
        {
            //benötigte Veriablen und Listen
            bool allowToCreatePoll = false;
            var user = await userManager.GetUserAsync(HttpContext.User);
            IList<string> roles = await userManager.GetRolesAsync(user);
            List<Poll> polls = null;
            List<SelectListItem> categoriesList = new List<SelectListItem>();
            List<SelectListItem> statusList = new List<SelectListItem>();
            List<Category> categories = db.Category.ToList();
            List<AnswerOptions> answerOptions = db.AnswerOptions.ToList();
            //Status Liste für Filter füllen
            statusList.Add(new SelectListItem { Value = "0", Text = "Alle Umfragen" });
            statusList.Add(new SelectListItem { Value = "2", Text = "laufende Umfragen" });
            statusList.Add(new SelectListItem { Value = "3", Text = "beendete Umfragen" });
            categoriesList.Add(new SelectListItem { Value = "0", Text = "Alle Kategorien" });
            //Rollenspezifische Variablen füllen, Umfragen je nach Rolle selektieren
            if (roles[0] == "Verw")
            {
                allowToCreatePoll = true;
                polls = db.Poll.Where(p => p.End >= DateTime.Today.AddMonths(-1)).OrderByDescending(p => p.End)
                    .ToList();
            }

            if (roles[0] == "BM" || roles[0] == "GR")
            {
                polls = db.Poll.Where(p => p.End >= DateTime.Today.AddMonths(-1)).OrderByDescending(p => p.End)
                    .ToList();
            }

            if (roles[0] == "Admin")
            {
                polls = db.Poll.OrderByDescending(p => p.End).ToList();
                statusList.Add(new SelectListItem { Value = "6", Text = "abgeschlossene Umfragen" });
            }

            foreach (Category category in categories)
            {
                categoriesList.Add(new SelectListItem { Value = category.Id.ToString(), Text = category.Description });
            }

            //Status der Umfragen ändern, falls nötig
            foreach (Poll poll in polls)
            {
                if (poll.End <= DateTime.Today.AddMonths(-1))
                {
                    poll.StatusId = 6;
                    db.Update(poll);
                    continue;
                }

                if (poll.End <= DateTime.Today)
                {
                    poll.StatusId = 3;
                    db.Update(poll);
                }
            }

            //Zusätzliche Variablen für das Rendern der Ansicht füllen
            ViewData["CategoriesList"] = categoriesList;
            ViewData["StatusList"] = statusList;
            ViewData["allowToCreatePoll"] = allowToCreatePoll;
            db.SaveChanges();
            return View("PollsAdministrationView", polls);
        }
        //Gibt Daten einer Umfrage als JSON String zurück
        [HttpGet]
        public IActionResult GetPollJson(int pollId)
        {
            int votes = 0;
            // Umfrage und Antworten der Umfrage selektieren
            Poll poll = db.Poll.Where(p => p.Id == pollId).SingleOrDefault();
            List<AnswerOptionsPoll> answerOptionsPolls =
                db.AnswerOptionsPoll.Where(aop => aop.PollId == pollId).ToList();
            //Anzahl der Einwohner die Abgestimmt haben berechnen
            foreach (AnswerOptionsPoll answerOptionsPoll in answerOptionsPolls)
            {
                votes += db.UserAnswerOptionsPoll.Where(uaop => uaop.AnswerOptionsPollId == answerOptionsPoll.Id)
                    .Count();
            }

            return Json(new { title = poll.Title, text = poll.Text, end = poll.End, votes });
        }
        //Gibt Antworten und die Anzahl der Stimmen der jeweiligen Antwort einer Umfrage als JSON String zurück
        public IActionResult GetPollAnswers(int pollId)
        {
            int id = pollId;
            // Umfrage und Antworten selektieren
            Poll poll = db.Poll.Where(p => p.Id == id).SingleOrDefault();
            List<AnswerOptionsPoll> answerOptionsPolls = db.AnswerOptionsPoll.Where(aop => aop.PollId == poll.Id)
                .Include("AnswerOptions").Where(aop => aop.AnswerOptionsId == aop.AnswerOptions.Id).ToList();
            foreach (AnswerOptionsPoll answerOptionsPoll in answerOptionsPolls)
            {
                List<UserAnswerOptionsPoll> userAnswerOptionsPolls = db.UserAnswerOptionsPoll
                    .Where(uaop => uaop.AnswerOptionsPollId == answerOptionsPoll.Id).ToList();
                answerOptionsPoll.UserAnswerOptionsPoll = userAnswerOptionsPolls;
            }

            poll.AnswerOptionsPoll = answerOptionsPolls;

            return poll.getAnswers();
        }
        //Zeigt die Ansicht Kategorien verwalten an
        [Authorize(Roles = "Admin")]
        public IActionResult ShowCategories()
        {
            List<Category> categories = db.Category.ToList();
            return View("CategoriesAdministrationView", categories);
        }
        //Verwaltung einer Kategorie
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult crudCategory(string categoryId, string description, string operation)
        {
            //Variablen
            Category category = new Category();
            string title = "";
            string text = "";
            int result = 0;
            int categoryIdInt = Convert.ToInt32(categoryId);
            try
            {
                //Update bzw. Änderung der Beschreibung
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
                //neue Kategorie erstellen
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
                //Kategorie löschen
                if (Equals(operation, "delete"))
                {
                    result = 3;
                }
            }
            catch (Exception e)
            {
                title = "Fehler";
                text = e.Message;
                result = 0;
            }

            return Json(new { result, title, text, description, categoryId = categoryIdInt });
        }

        //Zeigt die Ansicht User Verwalten an
        [Authorize(Roles = "Admin")]
        public IActionResult ShowUsers()
        {
            //Variablen
            List<User> users = db.User.Include("UserRole").ToList();
            List<SelectListItem> rolesList = new List<SelectListItem>();
            List<Role> roles = db.Role.ToList();
            //Anzeige der Rolle je nach sperren oder anpassen
            foreach (Role role in roles)
            {
                bool disabled = false;
                string description = "";
                switch (role.Name)
                {
                    //Zählen der GR Mitglieder
                    case "GR":
                        int count = db.UserRole.Where(ur => ur.RoleId == role.Id).Count();
                        if (count >= 35) disabled = true;
                        description = role.Description + "(" + count + "/35)";
                        break;
                    //Nur eine Person darf die Rolle Bürgermeister haben
                    case "BM":
                        if (db.UserRole.Where(ur => ur.RoleId == role.Id).Count() >= 1) disabled = true;
                        description = role.Description;
                        break;
                    default:
                        description = role.Description;
                        break;
                }

                rolesList.Add(new SelectListItem { Value = role.Id.ToString(), Text = description, Disabled = disabled });
            }

            ViewData["Roles"] = rolesList;
            return View("UsersAdministrationView", users);
        }
        //Gibt alle Daten eines Users als JSON String zurück
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetUser(int userId)
        {
            //int userIdInt = Convert.ToInt32(userId);
            User user = db.User.Where(u => u.Id == userId).Include("Address").Include("UserRole").SingleOrDefault();
            user.Password = "";
            return Json(user);
        }
        //User verwalten
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult crudUser(User user, int roleId, string operation)
        {
            //Variablen
            string title = "";
            string text = "";
            string localCouncilDescription = "";
            bool localCouncilFull = false;
            bool mayorFull = false;
            int localCouncilId = 4;
            int mayorId = 3;
            int result = 0;
            //User aus DB selektieren und updaten
            User dbUser = db.User.Where(u => u.Id == user.Id).SingleOrDefault();
            dbUser.Firstname = user.Firstname;
            dbUser.Name = user.Name;
            dbUser.Birthplace = user.Birthplace;
            dbUser.Birthday = user.Birthday;
            dbUser.Email = user.Email;
            db.Update(dbUser);
            //Neue Rolle zuweisen, falls diese geändert wurde
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
            localCouncilDescription = localCouncilDescription + "(" + count + "/35)";
            if (count >= 35) localCouncilFull = true;
            //BM zählen
            if (db.UserRole.Where(ur => ur.RoleId == mayorId).Count() >= 1) mayorFull = true;
            return Json(new { result, user, roleId, title, text, localCouncilDescription, localCouncilFull, mayorFull });
        }
        //Gibt die Ansicht zur Erstellung der Registierungsschlüssel mit einem neuen Registrierungsschlüssels zurück
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult GetRandomKey()
        {
            Randomkey key = GenerateRandomkey();
            var dbCheck = db.Randomkey.Where(r => r.Key == key.Key).ToList();
            if (dbCheck.Count == 0)
            {
                db.Randomkey.Add(key);
                db.SaveChanges();
                return View("RandomKeyView", key);
            }

            GetRandomKey();
            throw new Exception("Key wird bereits verwendet ");
        }
        //Erstellung eines Registierungsschlüssels nach dem Zufallsprinizp aus vorgegeben Zeichen
        private Randomkey GenerateRandomkey()
        {
            Random random = new Random();
            string characters = "123456789ABCDEFGHIJKLMNPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            StringBuilder result = new StringBuilder(11);
            for (int i = 0; i < 11; i++)
            {
                result.Append(characters[random.Next(characters.Length)]);
            }

            Randomkey key = new Randomkey();
            string strresult = result.ToString();
            key.Key = strresult;
            return key;
        }
        //Erstellung einer Worddatei mit Anschreiben und einem Registrierungsschlüssel für die Einwohner
        public IActionResult GenerateWord(string key)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            string path = webRootPath + @"\AnschreibenEinwohner.docx";
            WordDocument document = new WordDocument();
            document.EnsureMinimal();
            document.LastParagraph.AppendText(key);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, FormatType.Docx);
            stream.Position = 0;
            return File(stream, "application/msword", "Registrierung.docx");
            // string webRootPath = _hostingEnvironment.WebRootPath;
            //string path = webRootPath+@"\AnschreibenEinwohner.docx";


            //return null;
            //Bookmark bm = doc.Bookmarks[bookmark];
            //Range range = bm.Range;
            //range.Text = key;
            //doc.Bookmarks.Add(bookmark, range);
        }
        //Zeigt die Ansicht zur Erstellung der Registierungsschlüssel an
        [Authorize(Roles = "Admin")]
        public IActionResult ShowKey()
        {
            Randomkey key = new Randomkey();
            key.Id = -1;
            key.Key = "XXXXXXXXXXX";
            return View("RandomKeyView", key);
        }
    }
}