namespace mosPortal.Models
{
    public class UserConcern
    {
        public int UserId { get; set; }
        public int ConcernId { get; set; }

        public Concern Concern { get; set; }
        public User User { get; set; }
    }
}