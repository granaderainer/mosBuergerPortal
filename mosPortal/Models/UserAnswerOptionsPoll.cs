using System;
using System.Collections.Generic;

namespace mosPortal.Models
{
    public partial class UserAnswerOptionsPoll
    {
        public int UserId { get; set; }
        public int AnswerOptionsPollId { get; set; }

        public AnswerOptionsPoll AnswerOptionsPoll { get; set; }
        public User User { get; set; }
    }
}
