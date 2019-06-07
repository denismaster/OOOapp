using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OOOVote.Data.Entities;

namespace OOOVote.Pages.Organizations
{
    public class AddUserModel : PageModel
    {
        private readonly OOOVote.Data.ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IEmailSender _emailSender;

        public AddUserModel(OOOVote.Data.ApplicationDbContext context, UserManager<User> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
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

            [Required]
            [Display(Name = "Роль")]
            public OrganizationRole Role { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Organization = await _context.Organizations.Include(o => o.Users).FirstOrDefaultAsync(m => m.Id == id);

            if (Organization == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Organization = await _context.Organizations.Include(o => o.Users).FirstOrDefaultAsync(m => m.Id == id);

            var existingUser = await _userManager.FindByEmailAsync(Input.Email);

            if (existingUser != null)
            {
                if (!Organization.Users.Any(u => u.UserId == existingUser.Id))
                {
                    Organization.Users.Add(new OrganizationUser { OrganizationId = Organization.Id, UserId = existingUser.Id, OrganizationRole = Input.Role });
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return RedirectToPage("./Details", new { Id = Organization.Id });
                }
                
            }
            else
            {
                var invitationCode = new InvitationCode
                {
                    Code = Guid.NewGuid(),
                    CreateDate = DateTime.UtcNow,
                    Email = Input.Email,
                    InitialRole = Input.Role,
                    Organization = Organization
                };

                _context.InvitationCodes.Add(invitationCode);
                await _context.SaveChangesAsync();

                var callbackUrl = Url.Page(
                    "/Account/Register",
                    pageHandler: null,
                    values: new { area = "Identity", code = invitationCode.Code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(
                    Input.Email,
                    $"Приглашение в {Organization.Name}",
                    $"Зарегистрируйтесь, перейдя по <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>ссылке</a>.");
            }

            return RedirectToPage("./Details", new { Id = Organization.Id });
        }

    }
}
