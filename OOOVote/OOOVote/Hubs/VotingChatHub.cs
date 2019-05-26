using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using OOOVote.Data;
using OOOVote.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OOOVote.Hubs
{
    [Authorize]
    public class VotingChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public VotingChatHub(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task SendMessage(string groupName, string message)
        {
            var user = await _userManager.GetUserAsync(Context.User);

            if (!Guid.TryParse(groupName, out Guid votingId))
                return;

            await Clients.Group(groupName).SendAsync("ReceiveMessage", $"{user.LastName} {user.FirstName}", message);

            _context.VotingMessages.Add(new VotingMessage
            {
                CreatedDate = DateTime.UtcNow,
                UserId = user.Id,
                VotingId = votingId,
                Message = message
            });

            await _context.SaveChangesAsync();
        }
    }
}
