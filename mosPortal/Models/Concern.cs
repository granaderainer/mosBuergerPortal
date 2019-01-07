using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace mosPortal.Models
{
    public partial class Concern
    {
        public Concern()
        {
            Comment = new HashSet<Comment>();
            UserConcern = new HashSet<UserConcern>();
        }

        public int Id { get; set; }
        [Required(ErrorMessage = "Bitte geben Sie einen Text ein")]
        [Display(Name = "Text/Beschreibung")]
        public string Text { get; set; }
        public int UserId { get; set; }
        public DateTime? Date { get; set; }
        [Required(ErrorMessage = "Bitte geben Sie einen Titel ein")]
        [Display(Name = "Titel")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Kategorie")]
        public int CategoryId { get; set; }
        
        public int StatusId { get; set; }

        public Category Category { get; set; }
        public User User { get; set; }
        public ICollection<Comment> Comment { get; set; }
        [Display(Name = "Likes")]
        public ICollection<UserConcern> UserConcern { get; set; }
        [Display(Name = "Status")]
        public Status Status { get; set; }

        public int vote(int userID)
        {
            UserConcern.Add(new Models.UserConcern
            {
                UserId = userID,
                ConcernId = this.Id
            });
            int votes = UserConcern.Count;
            return votes;
        }
        public Boolean allowToVote(User user)
        {
            foreach (var uc in UserConcern)
            {
                if (uc.UserId == user.Id)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
