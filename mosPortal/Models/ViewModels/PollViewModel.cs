using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace mosPortal.Models.ViewModels
{
    public class PollViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Ablaufdatum")]
        public DateTime? End { get; set; }
        public int UserId { get; set; }
        [Required]
        [Display(Name = "Abstimmung Gemeinderat")]
        public bool? NeedsLocalCouncil { get; set; }
        public bool? Approved { get; set; }
        [Required]
        [Display(Name = "Kategorie")]
        public int CategoryId { get; set; }
        [Required]
        [Display(Name = "Titel")]
        public string Title { get; set; }
        public Category Category { get; set; }
        public User User { get; set; }
        public ICollection<AnswerOptionsPoll> AnswerOptionsPoll { get; set; }
        public int RadioId { get; set; }
    }
}
