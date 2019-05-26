using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OOOVote.Data.Entities
{
    public class InvitationCode
    {
        public Guid Id { get; set; }
        public Guid Code { get; set; }
        public DateTime CreateDate { get; set; }
        public OrganizationRole InitialRole { get; set; }
        public string Email { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}
