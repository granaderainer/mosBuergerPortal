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
        [Display(Name = "RadioId")]
        public string RadioId { get; set; }

    }
}
