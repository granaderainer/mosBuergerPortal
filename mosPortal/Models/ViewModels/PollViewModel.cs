using System;
using System.Collections.Generic;

namespace mosPortal.Models.ViewModels
{
    public class PollViewModel
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public DateTime End { get; set; }
        public int UserId { get; set; }

        public bool? NeedsLocalCouncil { get; set; }
        public bool? Approved { get; set; }

        public int CategoryId { get; set; }

        public string Title { get; set; }
        public Category Category { get; set; }
        public User User { get; set; }
        public ICollection<AnswerOptionsPoll> AnswerOptionsPoll { get; set; }
        public int RadioId { get; set; }
        public int userAnswerOptionsPollId { get; set; }
        public ICollection<Image> Image { get; set; }
        public ICollection<File> File { get; set; }
    }
}