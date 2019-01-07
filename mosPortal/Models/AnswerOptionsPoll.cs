using System;
using System.Collections.Generic;

namespace mosPortal.Models
{
    public partial class AnswerOptionsPoll
    {
        public AnswerOptionsPoll()
        {
            UserAnswerOptionsPoll = new HashSet<UserAnswerOptionsPoll>();
        }

        public int AnswerOptionsId { get; set; }
        public int PollId { get; set; }
        public int Id { get; set; }
        public AnswerOptions AnswerOptions { get; set; }
        public Poll Poll { get; set; }
        public ICollection<UserAnswerOptionsPoll> UserAnswerOptionsPoll { get; set; }
    }
}
