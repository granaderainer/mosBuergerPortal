using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mosPortal.Models.ViewModels
{
    public class CreatePollModel
    {
        public string Text { get; set; }
        public DateTime End { get; set; }
        public bool NeedsLocalCouncil { get; set; }
        public string Title { get; set; }
        public string[] Answers { get; set; }
        public int ConcernId { get; set; }
        public int CategoryId { get; set; }

        public int[] ImageIds { get; set; }
        public int[] FileIds { get; set; }
    }
}
