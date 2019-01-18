using System;
using System.Collections.Generic;

namespace mosPortal.Models
{
    public partial class Image
    {
        public int Id { get; set; }
        public byte[] Img { get; set; }
        public int? ConcernId { get; set; }
        public int? PollId { get; set; }
        public string Name { get; set; }
        public string Ending { get; set; }
        public Concern Concern { get; set; }
        public Poll Poll { get; set; }
    }
}
