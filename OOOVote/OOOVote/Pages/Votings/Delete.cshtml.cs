using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OOOVote.Data;
using OOOVote.Data.Entities;

namespace OOOVote.Pages
{
    public class DeleteModel : PageModel
    {
        private readonly OOOVote.Data.ApplicationDbContext _context;

        public DeleteModel(OOOVote.Data.ApplicationDbContext context)
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
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Voting = await _context.Votings.FindAsync(id);

            if (Voting != null)
            {
                _context.Votings.Remove(Voting);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
