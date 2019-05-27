using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OOOVote.Data.Entities
{
    public class UserVotingDecision
    {
        public Guid Id { get; set; }
        
        public VoteDecision Decision { get; set; }

        public Guid VotingOptionId { get; set; }
        public VotingOption VotingOption { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
