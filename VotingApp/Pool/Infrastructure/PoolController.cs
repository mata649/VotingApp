using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingApp.Base.Domain;
using VotingApp.Rest;
using VotingApp.Pool.Domain;
using System.Linq.Expressions;
using VotingApp.User.Domain;
using VotingApp.Pool.Domain.DTO;

namespace VotingApp.Pool.Rest;

[Route("api/pools")]
[ApiController]
public class PoolController : ControllerBase
{
    private readonly IPoolService _service;

    public PoolController(IPoolService poolService)
    {
        _service = poolService;
    }

    // GET: api/pools
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

    // GET api/pools/5
    [HttpGet("{id}"), Authorize]
    public ActionResult Get(Guid id)
    {
        var resp = _service.GetById(new() { ID = id });
        return StatusCode(resp.Type, resp.Value);
    }

    // POST api/pools
    [HttpPost, Authorize]
    public ActionResult Post([FromBody] CreatePoolDTO createPoolDTO)
    {
        createPoolDTO.UserID = JWT.GetCurrentUserID(HttpContext);
        var resp = _service.Create(createPoolDTO);
        return StatusCode(resp.Type, resp.Value);
    }

    // PUT api/pools/5
    [HttpPut("{id}"), Authorize]
    public ActionResult Put(Guid id, [FromBody] UpdatePoolDTO updatePoolDTO)
    {
        updatePoolDTO.CurrentUserID = JWT.GetCurrentUserID(HttpContext);

        updatePoolDTO.ID = id;
        var resp = _service.Update(updatePoolDTO);
        return StatusCode(resp.Type, resp.Value);
    }

    // DELETE api/pools/5
    [HttpDelete("{id}"), Authorize]
    public ActionResult Delete(Guid id)
    {
        var resp = _service.Delete(new() { ID = id, CurrentUserID = JWT.GetCurrentUserID(HttpContext) });
        return StatusCode(resp.Type, resp.Value);
    }



}

