using Microsoft.AspNetCore.Identity;
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
        public User (int id)
        {
            this.Id = id;
        }

        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Birthplace { get; set; }
        public DateTime Birthday { get; set; }
        public int AddressId { get; set; }
        public string Password { get; set; }

        public Address Address { get; set; }
        public ICollection<Comment> Comment { get; set; }
        public ICollection<Concern> Concern { get; set; }
        public ICollection<Poll> Poll { get; set; }
        public ICollection<UserAnswerOptionsPoll> UserAnswerOptionsPoll { get; set; }
        public ICollection<UserConcern> UserConcern { get; set; }

        public ICollection<UserRole> UserRole { get; set; }
        public ICollection<Concern> ConcernLastUpdatedByUser { get; set; }
        public ICollection<Poll> PollLastUpdatedByUser { get; set; }

        public Boolean allowToVote(int concernId)
        {
            foreach (var uc in UserConcern)
            {
                if (uc.ConcernId == concernId)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
