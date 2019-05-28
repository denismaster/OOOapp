using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OOOVote.Data;
using OOOVote.Data.Entities;

namespace OOOVote.Pages
{
    public class EditModel : PageModel
    {
        private readonly OOOVote.Data.ApplicationDbContext _context;

        public EditModel(OOOVote.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Voting Voting { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Voting = await _context.Votings
                .Include(v => v.Organization).FirstOrDefaultAsync(m => m.Id == id);

            if (Voting == null)
            {
                return NotFound();
            }
           ViewData["OrganizationId"] = new SelectList(_context.Organizations, "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Voting).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VotingExists(Voting.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool VotingExists(Guid id)
        {
            return _context.Votings.Any(e => e.Id == id);
        }
    }
}
