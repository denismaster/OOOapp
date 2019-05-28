using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OOOVote.Data;
using OOOVote.Data.Entities;

namespace OOOVote.Pages
{
    public class CreateModel : PageModel
    {
        private readonly OOOVote.Data.ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        [BindProperty]
        public List<string> Options { get; set; } = new List<string> { "" };

        public CreateModel(OOOVote.Data.ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            if (!Guid.TryParse(_userManager.GetUserId(User), out Guid userId))
            {
                return BadRequest();
            }

            var organizations = _context.Organizations
                .Include(o => o.Users)
                .Where(o => o.Users.Any(ou => ou.UserId == userId));

            ViewData["OrganizationId"] = new SelectList(organizations, "Id", "Name");
            return Page();
        }

        [BindProperty]
        public Voting Voting { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Voting.VotingOptions == null)
            {
                Voting.VotingOptions = new List<VotingOption>();
            }

            var votingOptions = Options
                .Select(o => new VotingOption
                {
                    Title = o
                })
                .ToList();

            Voting.VotingOptions = votingOptions;

            _context.Votings.Add(Voting);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
