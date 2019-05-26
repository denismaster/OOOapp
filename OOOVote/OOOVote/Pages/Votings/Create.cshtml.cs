using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using OOOVote.Data;
using OOOVote.Data.Entities;

namespace OOOVote.Pages
{
    public class CreateModel : PageModel
    {
        private readonly OOOVote.Data.ApplicationDbContext _context;

        public CreateModel(OOOVote.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["OrganizationId"] = new SelectList(_context.Organizations, "Id", "Id");
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

            _context.Votings.Add(Voting);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}