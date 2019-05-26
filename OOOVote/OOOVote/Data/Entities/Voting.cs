using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OOOVote.Data.Entities
{
    public class Voting
    {
        public Guid Id { get; set; }
        
        public string Subject { get; set; }
        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public List<VotingMessage> VotingMessages { get; set; }
        public List<VotingOption> VotingOptions { get; set; }
    }
}
