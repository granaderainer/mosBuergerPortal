using System;
using System.IO;
using System.Linq;
using mosPortal.Data;
using mosPortal.Models;
using Microsoft.AspNetCore.Mvc;
using File = mosPortal.Models.File;

namespace mosPortal.Controllers
{
    public class FileController : Controller
    {
        private readonly dbbuergerContext db;
        public FileController(dbbuergerContext db)
        {
            this.db = db;
        }
        public FileStreamResult GetImage(int id)
        {
            Image image = db.Image.Where(i => i.Id == id).SingleOrDefault();
//            if (image == null)
//            {
//                image = db.Image.Where(i => i.Id == 18).SingleOrDefault();
//            }
            Stream imageStream = new MemoryStream(image.Img);
            return new FileStreamResult(imageStream, image.Ending);
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
                image = db.Image.Where(i => i.Id == 18).SingleOrDefault();
            }

            Stream imageStream = new MemoryStream(image.Img);
            return new FileStreamResult(imageStream, image.Ending);
        }

        public FileStreamResult GetFile(int id)
        {
            File file = db.File.Where(f => f.Id == id).SingleOrDefault();
            Stream fileStream = new MemoryStream(file.File1);
            return new FileStreamResult(fileStream, file.Ending);
        }
    }
}