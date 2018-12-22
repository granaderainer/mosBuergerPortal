﻿using System;
using System.Collections.Generic;

namespace mosPortal.Models
{
    public partial class AnswerOptions
    {
        public AnswerOptions()
        {
            AnswerOptionsPoll = new HashSet<AnswerOptionsPoll>();
        }

        public int Id { get; set; }
        public string Description { get; set; }

        public ICollection<AnswerOptionsPoll> AnswerOptionsPoll { get; set; }
    }
}
