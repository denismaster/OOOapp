using System;
using System.Collections.Generic;

namespace OOOVote.Data.Entities
{
    public class VotingOption
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public Guid VotingId { get; set; }
        public Voting Voting { get; set; }

        public List<UserVotingDecision> Decisions { get; set; }
    }
}
