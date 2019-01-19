using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using mosPortal.Data;
using mosPortal.Models;

namespace mosPortal.Controllers
{
    public class ImageController : Controller
    {
        private dbbuergerContext db = new dbbuergerContext();
        public FileStreamResult GetImage(int id)
        {
            Image image = db.Image.Where(i => i.Id == id).SingleOrDefault();
            Stream imageStream = new MemoryStream(image.Img);
            return new FileStreamResult(imageStream, image.Ending);

            /*try
            {
                Image image = db.Image.Where(i => i.ConcernId == Convert.ToInt32(concernId)).First();
                Stream imageStream = new MemoryStream(image.Img);
                string fileType = image.Name.Split(".")[1];
                return new FileStreamResult(imageStream, image.Ending);
            }
            catch
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/admin_template/img/avatar-0.jpg");
                FileStream fs = new FileStream(path, FileMode.Open);
                byte[] byteData = new byte[fs.Length];
                fs.Read(byteData, 0, byteData.Length);
                fs.Close();
                Stream imageStream = new MemoryStream(byteData);
                return new FileStreamResult(imageStream, "image/jpeg");
            }*/

        }
        public FileStreamResult GetConcernTitleImage(int concernId)
        {
            Image image = null;
            try
            {
                image = db.Image.Where(i => i.ConcernId == Convert.ToInt32(concernId)).First();

            }
            catch
            {
                image = db.Image.Where(i => i.Id == 10).SingleOrDefault();
            }
            Stream imageStream = new MemoryStream(image.Img);
            return new FileStreamResult(imageStream, image.Ending);
        }
    }
}