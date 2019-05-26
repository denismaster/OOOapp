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
    public class DetailsModel : PageModel
    {
        private readonly OOOVote.Data.ApplicationDbContext _context;

        public DetailsModel(OOOVote.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Voting Voting { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Voting = await _context.Votings
                .Include(v => v.VotingMessages)
                    .ThenInclude(vm => vm.User)
                .Include(v => v.VotingOptions)
                .Include(v => v.Organization)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Voting == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
