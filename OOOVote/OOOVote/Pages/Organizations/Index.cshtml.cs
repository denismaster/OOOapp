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
    public class OrganizationListViewModel
    {
        public Guid Id { get; set; }
        public string OrganizationName { get; set; }
        public int UserCount { get; set; }
        public OrganizationRole Role { get; set; }
    }
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<OrganizationListViewModel> Organizations { get; set; } = new List<OrganizationListViewModel>();

        public async Task OnGetAsync()
        {
            if (!Guid.TryParse(_userManager.GetUserId(User), out Guid id))
            {
                return;
            }

            var user = await _context.Users
                .Include(u => u.Organizations)
                .ThenInclude(uo => uo.Organization)
                .ThenInclude(o => o.Users)
                .FirstOrDefaultAsync(u => u.Id == id);

            Organizations = user.Organizations
                .Select(uo => new OrganizationListViewModel
                {
                    Id = uo.OrganizationId,
                    OrganizationName = uo.Organization.Name,
                    Role = uo.OrganizationRole,
                    UserCount = uo.Organization.Users.Count
                })
                .ToList();
        }
    }
}
