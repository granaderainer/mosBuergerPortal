using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mosPortal.Models
{
    public class Concern
    {
        public Concern()
        {
            Comment = new HashSet<Comment>();
            UserConcern = new HashSet<UserConcern>();
            Image = new HashSet<Image>();
            File = new HashSet<File>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie einen Text ein")]
        [Display(Name = "Text/Beschreibung")]
        public string Text { get; set; }

        public int UserId { get; set; }
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie einen Titel ein")]
        [Display(Name = "Titel")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Kategorie")]
        public int CategoryId { get; set; }

        public int StatusId { get; set; }

        public int? LastUpdatedBy { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public string AdminComment { get; set; }

        public Category Category { get; set; }
        public User User { get; set; }
        public User LastUpdatedByUser { get; set; }
        public ICollection<Comment> Comment { get; set; }

        [Display(Name = "Likes")] public ICollection<UserConcern> UserConcern { get; set; }

        public ICollection<Image> Image { get; set; }
        public ICollection<File> File { get; set; }

        [Display(Name = "Status")] public Status Status { get; set; }

        //Erstellt einen Vote für einen User und gibt die Anzahl der Votes zurück
        public int vote(int userID)
        {
            UserConcern.Add(new UserConcern
            {
                UserId = userID,
                ConcernId = Id
            });
            int votes = UserConcern.Count;
            return votes;
        }
        //Prüft ob ein User berechtigt ist zu voten oder bereits gevotet hat
        public bool allowToVote(User user)
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