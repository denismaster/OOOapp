using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OOOVote.Data;
using OOOVote.Data.Entities;

namespace OOOVote.Pages.Organizations
{
    public class CreateModel : PageModel
    {
        private readonly OOOVote.Data.ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public CreateModel(OOOVote.Data.ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Organization Organization { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!Guid.TryParse(_userManager.GetUserId(User), out Guid id))
            {
                return Page();
            }

            _context.Organizations.Add(Organization);

            await _context.SaveChangesAsync();

            Organization.Users.Add(new OrganizationUser
            {
                UserId = id,
                OrganizationId = Organization.Id,
                OrganizationRole = Organization.RuleType == RuleType.Single ? OrganizationRole.CEO : OrganizationRole.TopManager
            });

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}