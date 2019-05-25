using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OOOVote.Data.Entities
{
    public enum RuleType
    {
        Single,
        Collective
    }

    public enum VotingRules
    {
        Proportional,
        NonProportional
    }

    public class Organization
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DateFounded { get; set; }
        public decimal ShareСapital { get; set; }
        public string RulesUrl { get; set; }

        public RuleType RuleType { get; set; }

        public List<OrganizationUser> Users { get; set; } = new List<OrganizationUser>();
    }
}
