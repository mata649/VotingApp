using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using VotingApp.Vote.Domain;
using VotingApp.Vote.Domain.DTO;

namespace VotingApp.Infrastructure
{
    public class DashboardHub : Hub
    {
        private readonly IVoteNotificationService _notificationService;

        public DashboardHub(IVoteNotificationService notificationService)
        {
            _notificationService = notificationService;
        }


        public async Task JoinDashboard(Guid pollID)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, pollID.ToString());
            var countResp = _notificationService.GetCountOfVotes(pollID);
            if (countResp.Type == 200 && countResp.Value is List<CountByOptionDTO> count) await Clients.Group(pollID.ToString())
                    .SendAsync("AwaitDashboardInfo", JsonSerializer.Serialize(count));

        }

        public async Task ExitGroup(string pollID)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, pollID.ToString());
        }

    }
}
