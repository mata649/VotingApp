using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using VotingApp.Base.Domain;
using VotingApp.Infrastructure;
using VotingApp.Option.Domain;
using VotingApp.Rest;
using VotingApp.Vote.Domain;
using VotingApp.Vote.Domain.DTO;

namespace VotingApp.Vote.Infrastructure
{
    [Route("api/votes")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        private readonly IVoteService _service;
        private readonly IHubContext<DashboardHub> _hubContext;
        private readonly IVoteNotificationService _notificationService;
        public VoteController(IVoteService voteService, IHubContext<DashboardHub> hubContext, IVoteNotificationService notificationService)
        {
            _service = voteService;
            _hubContext = hubContext;
            _notificationService = notificationService;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateVoteDTO createVoteDTO)
        {
            createVoteDTO.UserID = JWT.GetCurrentUserID(HttpContext);
            IResponse response = _service.Create(createVoteDTO);
            if (response.Type == 201 && response.Value is ResponseVoteDTO responseVote)
            {
                OptionEntity option = responseVote.Option;
                var countResp = _notificationService.AddVoteToDashboard(option);
                if (countResp.Type == 200 && countResp.Value is List<CountByOptionDTO> count) await _hubContext.
                        Clients.
                        Group(option.PollID.ToString()).
                        SendAsync("AwaitDashboardInfo", JsonSerializer.Serialize(count));
            }

            return StatusCode(response.Type, response.Value);

        }
    }
}
