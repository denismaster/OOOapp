using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OOOVote.Data;
using OOOVote.Data.Entities;

namespace OOOVote.Pages.Organizations
{
    public class OrganizationUserViewModel
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public OrganizationRole Role { get; set; }
    }
    public class DetailsModel : PageModel
    {
        private readonly OOOVote.Data.ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DetailsModel(OOOVote.Data.ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Organization Organization { get; set; }
        public List<OrganizationUserViewModel> Users { get; set; } = new List<OrganizationUserViewModel>();
        public bool CanEditOrganization { get; set; } = false;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!Guid.TryParse(_userManager.GetUserId(User), out Guid userId))
            {
                return NotFound();
            }

            Organization = await _context.Organizations
                .Include(u => u.Users)
                    .ThenInclude(ou => ou.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Organization == null)
            {
                return NotFound();
            }

            Users = Organization.Users
                .Select(ou => new OrganizationUserViewModel
                {
                    Id = ou.UserId,
                    FirstName = ou.User.FirstName,
                    LastName = ou.User.LastName,
                    Role = ou.OrganizationRole
                })
                .ToList();

            var currentRole = Organization.Users.FirstOrDefault(ou => ou.UserId == userId)?.OrganizationRole;

            if(currentRole == null || currentRole != OrganizationRole.CEO || currentRole!=OrganizationRole.TopManager)
            {
                CanEditOrganization = false;
            }
            else
            {
                CanEditOrganization = true;
            }

            return Page();
        }
    }
}
