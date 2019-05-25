using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OOOVote.Data;
using OOOVote.Data.Entities;

namespace OOOVote.Pages.Organizations
{
    public class DeleteModel : PageModel
    {
        private readonly OOOVote.Data.ApplicationDbContext _context;

        public DeleteModel(OOOVote.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Organization Organization { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Organization = await _context.Organizations.FirstOrDefaultAsync(m => m.Id == id);

            if (Organization == null)
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

            Organization = await _context.Organizations.FindAsync(id);

            if (Organization != null)
            {
                _context.Organizations.Remove(Organization);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
