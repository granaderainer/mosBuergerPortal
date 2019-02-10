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
        //Attribute
        private readonly dbbuergerContext db;
        //Konstruktor
        public FileController(dbbuergerContext db)
        {
            this.db = db;
        }
        //Bild aus DB ins Memory laden um es anzuzeigen
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
        //Erstes Bild für Concern aus DB ins Memory laden um es als Titelbild anzuzeigen
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
        //Datei aus DB ins Memory laden um es anzuzeigen
        public FileStreamResult GetFile(int id)
        {
            File file = db.File.Where(f => f.Id == id).SingleOrDefault();
            Stream fileStream = new MemoryStream(file.File1);
            return new FileStreamResult(fileStream, file.Ending);
        }
    }
}