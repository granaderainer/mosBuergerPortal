using System;
using System.Collections.Generic;

namespace mosPortal.Models
{
    public partial class User
    {
        public User()
        {
            Comment = new HashSet<Comment>();
            Concern = new HashSet<Concern>();
            Poll = new HashSet<Poll>();
            UserAnswerOptionsPoll = new HashSet<UserAnswerOptionsPoll>();
            UserConcern = new HashSet<UserConcern>();
        }
        public User(int id)
        {
            this.Id = id;

            // User aus DB!
        }


        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Birthplace { get; set; }
        public string Birthday { get; set; }
        public int AdressId { get; set; }
        public int UserroleId1 { get; set; }

        public Address Adress { get; set; }
        public Userrole UserroleId1Navigation { get; set; }
        public ICollection<Comment> Comment { get; set; }
        public ICollection<Concern> Concern { get; set; }
        public ICollection<Poll> Poll { get; set; }
        public ICollection<UserAnswerOptionsPoll> UserAnswerOptionsPoll { get; set; }
        public ICollection<UserConcern> UserConcern { get; set; }
    }
}
