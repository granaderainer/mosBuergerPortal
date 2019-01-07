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
        public byte? NeedsLocalCouncil { get; set; }
        public byte? Approved { get; set; }

        public User User { get; set; }
        public ICollection<AnswerOptionsPoll> AnswerOptionsPoll { get; set; }

    }
}
