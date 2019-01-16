using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mosPortal.Models
{
    public partial class Poll
    {
        public Poll()
        {
            AnswerOptionsPoll = new HashSet<AnswerOptionsPoll>();
            Image = new HashSet<Image>();
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
        [Required]
        [Display(Name = "Titel")]
        public string Title { get; set; }

        public int? LastUpdatedBy { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

        public int? StatusId { get; set; }

        public Status Status { get; set; }

        public Category Category { get; set; }
        public User User { get; set; }
        public User LastUpdatedByUser { get; set; }
        public ICollection<AnswerOptionsPoll> AnswerOptionsPoll { get; set; }
        public ICollection<Image> Image { get; set; }
        public class Answer
        {
            public int value { get; set; }
            public String label { get; set; }
        }
        /*public List<Answer> getAnswers()
        {
            List<Answer> answers = new List<Answer>();
            foreach (AnswerOptionsPoll aop in AnswerOptionsPoll)
            {
                Answer answer = new Answer
                {
                    Votes = aop.UserAnswerOptionsPoll.Count,
                    AnswerDescription = aop.AnswerOptions.Description

                };
                answers.Add(answer);

            }
            return answers;
        }*/
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


    }
}
