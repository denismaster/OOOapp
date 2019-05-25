using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OOOVote.Data.Entities;

namespace OOOVote.Pages.Organizations
{
    public class AddUserModel : PageModel
    {
        private readonly OOOVote.Data.ApplicationDbContext _context;

        public AddUserModel(OOOVote.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public Organization Organization { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
        }

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
    }
}