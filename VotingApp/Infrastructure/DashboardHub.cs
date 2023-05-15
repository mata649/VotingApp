using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using VotingApp.Vote.Domain;
using VotingApp.Vote.Domain.DTO;

namespace VotingApp.Infrastructure
{
    public class DashboardHub : Hub
    {
        private readonly IVoteService _voteService;

        public DashboardHub(IVoteService voteService)
        {
            _voteService = voteService;
        }


        public async Task JoinDashboard(Guid poolID)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, poolID.ToString());
            var countResp = _voteService.GetCountOfVotes(poolID);
            if (countResp.Type == 200 && countResp.Value is List<CountByOptionDTO> count) await Clients.Group(poolID.ToString())
                    .SendAsync("AwaitDashboardInfo", JsonSerializer.Serialize(count));

        }

        public async Task ExitGroup(string poolID)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, poolID.ToString());
        }

    }
}
