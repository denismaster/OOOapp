using System;

namespace OOOVote.Data.Entities
{
    public class OrganizationUser
    {
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public OrganizationRole OrganizationRole { get; set; }
    }
}
