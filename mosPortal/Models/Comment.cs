using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mosPortal.Models
{
    public class Comment
    {
        private int id;
        private String text;
        private String userId;

        public Comment(string text, string userId)
        {
            this.text = text;
            this.userId = userId;
        }

        public Comment(int id)
        {
            this.id = id;

            // DB Abfrage für User!
        }


    }
}
