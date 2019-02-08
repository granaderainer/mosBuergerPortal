using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace mosPortal.Models
{
    public class Poll
    {
        public Poll()
        {
            AnswerOptionsPoll = new HashSet<AnswerOptionsPoll>();
            Image = new HashSet<Image>();
            File = new HashSet<File>();
        }

        public int Id { get; set; }
        public string Text { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Ablaufdatum")]
        public DateTime? End { get; set; }

        public int UserId { get; set; }

        [Required]
        [Display(Name = "Abstimmung Gemeinderat")]
        public bool NeedsLocalCouncil { get; set; }

        public bool Approved { get; set; }

        [Required]
        [Display(Name = "Kategorie")]
        public int CategoryId { get; set; }

        [Required] [Display(Name = "Titel")] public string Title { get; set; }

        public int? LastUpdatedBy { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public int? StatusId { get; set; }

        public Status Status { get; set; }

        public Category Category { get; set; }
        public User User { get; set; }
        public User LastUpdatedByUser { get; set; }
        public ICollection<AnswerOptionsPoll> AnswerOptionsPoll { get; set; }
        public ICollection<Image> Image { get; set; }

        public ICollection<File> File { get; set; }

        //Gibt die Antworten und die Anzahl der Abstimmungen der Umfrage als JSON String zurück
        public JsonResult getAnswers()
        {
            List<Answer> answers = new List<Answer>();
            foreach (AnswerOptionsPoll aop in AnswerOptionsPoll)
            {
                Answer answer = new Answer
                {
                    value = aop.UserAnswerOptionsPoll.Count,
                    label = aop.AnswerOptions.Description
                };
                answers.Add(answer);
            }

            JsonResult json = new JsonResult(answers);
            return json;
        }

        public class Answer
        {
            public int value { get; set; }
            public string label { get; set; }
        }
    }
}