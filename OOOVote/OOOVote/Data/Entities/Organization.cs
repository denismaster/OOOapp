using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OOOVote.Data.Entities
{
    public class Organization
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DateFounded { get; set; }

        public List<OrganizationUser> Users { get; set; }
    }
}
