using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using OOOVote.Data;
using OOOVote.Data.Entities;

namespace OOOVote.Pages
{
    public class VotingResult
    {
        public int AgreeCount { get; set; }
        public int DisagreeCount { get; set; }
        public int NotVotedCount { get; set; }

        public int AllVotedCount { get; set; }

    }
    public class DetailsModel : PageModel
    {
        private readonly OOOVote.Data.ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public DetailsModel(OOOVote.Data.ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Voting Voting { get; set; }

        [BindProperty]
        public Dictionary<Guid, VoteDecision> UserVotes { get; set; } = new Dictionary<Guid, VoteDecision>();

        [BindProperty]
        public Dictionary<string, VotingResult> VotingResults { get; set; } = new Dictionary<string, VotingResult>();

        [BindProperty]
        public bool ShowResults { get; set; } = false;

        [BindProperty]
        public bool IsVotingEnded { get; set; } = false;

        [BindProperty]
        public int UserCount { get; set; } = 0;

        public async Task<IActionResult> OnGetAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!Guid.TryParse(_userManager.GetUserId(User), out Guid userId))
            {
                return BadRequest();
            }

            Voting = await _context.Votings
                .Include(v => v.VotingMessages)
                    .ThenInclude(vm => vm.User)
                .Include(v => v.VotingOptions)
                    .ThenInclude(v => v.Decisions)
                .Include(v => v.Organization)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Voting == null)
            {
                return NotFound();
            }

            UserCount = (await _context.Organizations
                .Include(o => o.Users)
                .FirstOrDefaultAsync(o => o.Id == Voting.OrganizationId))
                ?.Users?.Count ?? 0;

            UserVotes = Voting.VotingOptions.ToDictionary(v => v.Id, v => VoteDecision.NotVoted);

            var userDecisions = Voting.VotingOptions
                .SelectMany(v => v.Decisions)
                .Where(d => d.UserId == userId)
                .ToList();

            bool hasUserDecisions = userDecisions.Count >= Voting.VotingOptions.Count;

            ShowResults = hasUserDecisions || Voting.EndDate < DateTime.Now;

            IsVotingEnded = Voting.EndDate < DateTime.Now || Voting.VotingOptions
                .Select(v => v.Decisions)
                .All(d => d.Count >= UserCount * 0.51);

            if (ShowResults)
            {
                VotingResults = Voting.VotingOptions
                    .Select(vo => new { Title = vo.Title, Decisions = vo.Decisions })
                    .ToDictionary(x => x.Title, vx =>
                      {
                          return new VotingResult
                          {
                              AgreeCount = vx.Decisions.Count(x => x.Decision == VoteDecision.Agree),
                              DisagreeCount = vx.Decisions.Count(x => x.Decision == VoteDecision.Disagree),
                              NotVotedCount = vx.Decisions.Count(x => x.Decision == VoteDecision.NotVoted),
                              AllVotedCount = vx.Decisions.Count()
                          };
                      });
            }




            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!Guid.TryParse(_userManager.GetUserId(User), out Guid userId))
            {
                return BadRequest();
            }

            var decisions = UserVotes.Select(pair => new UserVotingDecision
            {
                Decision = pair.Value,
                VotingOptionId = pair.Key,
                UserId = userId
            });

            _context.VotingDecisions.AddRange(decisions);

            await _context.SaveChangesAsync();

            return await OnGetAsync(id);
        }
    }
}
