namespace mosPortal.Models
{
    public class Image
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