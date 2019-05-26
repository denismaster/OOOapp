using System;

namespace OOOVote.Data.Entities
{
    public class VotingMessage
    {
        public Guid Id { get; set; }

        public string Message { get; set; }
        public DateTime CreatedDate { get; set; }

        public Guid VotingId { get; set; }
        public Voting Voting { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
