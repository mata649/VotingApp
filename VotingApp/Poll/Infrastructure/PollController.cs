using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingApp.Base.Domain;
using VotingApp.Rest;
using VotingApp.Poll.Domain;
using VotingApp.Poll.Domain.DTO;

namespace VotingApp.Poll.Rest;

[Route("api/polls")]
[ApiController]
public class PollController : ControllerBase
{
    private readonly IPollService _service;

    public PollController(IPollService pollService)
    {
        _service = pollService;
    }

    // GET: api/polls
    [HttpGet, Authorize]
    public ActionResult Get(
        [FromQuery] string question = "",
        [FromQuery] Guid userID = default,
        [FromQuery] ushort pageSize = 10,
        [FromQuery] ushort currentPage = 1)
    {
        IResponse resp = _service.Get(new(currentPage, pageSize, question, userID));
        return StatusCode(resp.Type, resp.Value);
    }

    // GET api/polls/5
    [HttpGet("{id}"), Authorize]
    public ActionResult Get(Guid id)
    {
        var resp = _service.GetById(new() { ID = id });
        return StatusCode(resp.Type, resp.Value);
    }

    // POST api/polls
    [HttpPost, Authorize]
    public ActionResult Post([FromBody] CreatePollDTO createPollDTO)
    {
        createPollDTO.UserID = JWT.GetCurrentUserID(HttpContext);
        var resp = _service.Create(createPollDTO);
        return StatusCode(resp.Type, resp.Value);
    }

    // PUT api/polls/5
    [HttpPut("{id}"), Authorize]
    public ActionResult Put(Guid id, [FromBody] UpdatePollDTO updatePollDTO)
    {
        updatePollDTO.CurrentUserID = JWT.GetCurrentUserID(HttpContext);

        updatePollDTO.ID = id;
        var resp = _service.Update(updatePollDTO);
        return StatusCode(resp.Type, resp.Value);
    }

    // DELETE api/polls/5
    [HttpDelete("{id}"), Authorize]
    public ActionResult Delete(Guid id)
    {
        var resp = _service.Delete(new() { ID = id, CurrentUserID = JWT.GetCurrentUserID(HttpContext) });
        return StatusCode(resp.Type, resp.Value);
    }



}

