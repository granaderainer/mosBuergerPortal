using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            ViewData["ConcernStatusOneCount"] = db.Concern.Where(c=> c.StatusId == 1).Count();
            ViewData["ConcernStatusTwoCount"] = db.Concern.Where(c => c.StatusId == 2).Count();
            ViewData["ConcernStatusThreeCount"] = db.Concern.Where(c => c.StatusId == 3).Count();
            //Ablauf Datum!!!
            ViewData["PollCount"] = db.Poll.Count();
            return View("Index");
        }
        public IActionResult ShowConcerns(int statusId)
        {
            ViewData["Status"] = db.Status.Where(s => s.Id == statusId).SingleOrDefault().Description;
            List<Concern> concerns = db.Concern
                .Where(c=> c.StatusId == statusId)
                .Include("Category")
                .Where(c => c.CategoryId == c.Category.Id)
                .Include("Status")
                .Where(c=> c.StatusId == c.Status.Id)
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
        public JsonResult GetConcernJson(int concernId)
        {
            //Concern concern = db.Concern.Where(c => c.Id == concernId).Include("Status").Where(c=>c.StatusId == c.Status.Id).SingleOrDefault();
            Concern concern = db.Concern.Where(c => c.Id == concernId).SingleOrDefault();
            Status[] statuses = db.Status.ToArray();
            /*User user = await userManager.GetUserAsync(HttpContext.User);
            db.Add(new UserConcern
            {
                UserId = user.Id,
                ConcernId = concernId
            });
            await db.SaveChangesAsync();
            int votes = db.UserConcern.Where(uc => uc.ConcernId == concernId).Count();
            return Json(new { votes = votes });*/
            string statusesJson = Newtonsoft.Json.JsonConvert.SerializeObject(statuses);
            return Json(new { concernId, title = concern.Title, text= concern.Text,statusId = concern.StatusId ,date = concern.Date, statuses});
        }
        [HttpPost]
        public IActionResult ChangeConcernStatus(string concernModalStatus, string concernModalId)
        {
            int concernId = Convert.ToInt32(concernModalId);
            int statusId = Convert.ToInt32(concernModalStatus);
            Concern concern = db.Concern.Where(c => c.Id == concernId).SingleOrDefault();
            concern.StatusId = statusId;
            db.Concern.Update(concern);
            db.SaveChanges();
            return this.ShowConcerns(statusId);
        }
    }
}
