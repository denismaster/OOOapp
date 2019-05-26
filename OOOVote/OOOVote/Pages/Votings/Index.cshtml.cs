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
    public class IndexModel : PageModel
    {
        private readonly OOOVote.Data.ApplicationDbContext _context;

        public IndexModel(OOOVote.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Voting> Voting { get;set; }

        public async Task OnGetAsync()
        {
            Voting = await _context.Votings
                .Include(v => v.Organization).ToListAsync();
        }
    }
}
