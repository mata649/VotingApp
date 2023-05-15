using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotingApp.Base.Domain;
using VotingApp.Rest;
using VotingApp.Option.Domain;
using VotingApp.Option.Domain.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VotingApp.Option.Rest
{
    [Route("api/options")]
    [ApiController]
    public class OptionController : ControllerBase
    {
        private readonly IOptionService _service;

        public OptionController(IOptionService optionService)
        {
            _service = optionService;
        }

        // GET: api/options
        [HttpGet, Authorize]
        public ActionResult Get(
            [FromQuery] string text = "",
            [FromQuery] Guid poolID = default,
            [FromQuery] ushort pageSize = 10,
            [FromQuery] ushort currentPage = 1)
        {
            IResponse resp = _service.Get(new(currentPage, pageSize, text, poolID));
            return StatusCode(resp.Type, resp.Value);
        }

        // GET api/options/5
        [HttpGet("{id}"), Authorize]
        public ActionResult Get(Guid id)
        {
            var resp = _service.GetById(new() { ID = id });
            return StatusCode(resp.Type, resp.Value);
        }

        // POST api/options
        [HttpPost, Authorize]
        public ActionResult Post([FromBody] CreateOptionDTO createOptionDTO)
        {
            createOptionDTO.CurrentUserID = JWT.GetCurrentUserID(HttpContext);
            var resp = _service.Create(createOptionDTO);
            return StatusCode(resp.Type, resp.Value);
        }

        // PUT api/options/5
        [HttpPut("{id}"), Authorize]
        public ActionResult Put(Guid id, [FromBody] UpdateOptionDTO updateOptionDTO)
        {
            updateOptionDTO.CurrentUserID = JWT.GetCurrentUserID(HttpContext);

            updateOptionDTO.ID = id;
            var resp = _service.Update(updateOptionDTO);
            return StatusCode(resp.Type, resp.Value);
        }

        // DELETE api/options/5
        [HttpDelete("{id}"), Authorize]
        public ActionResult Delete(Guid id)
        {
            var resp = _service.Delete(new() { ID = id, CurrentUserID = JWT.GetCurrentUserID(HttpContext) });
            return StatusCode(resp.Type, resp.Value);
        }

    }
}
