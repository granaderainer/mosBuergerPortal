namespace mosPortal.Models
{
    public class File
    {
        public int Id { get; set; }
        public byte[] File1 { get; set; }
        public int? ConcernId { get; set; }
        public int? PollId { get; set; }

        public string Ending { get; set; }
        public string Name { get; set; }

        public Concern Concern { get; set; }
        public Poll Poll { get; set; }
    }
}