using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using VotingApp.Base.Domain;
using VotingApp.Rest;
using VotingApp.User.Domain;
using VotingApp.User.Domain.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VotingApp.User.Rest;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _service;
    private readonly JWT _jwt;

    public UserController(IUserService userService, JWT jwtGenerator)
    {
        _service = userService;
        _jwt = jwtGenerator;
    }

    // GET: api/users
    [HttpGet, Authorize]
    public ActionResult Get(
        [FromQuery] string name = "",
        [FromQuery] string email = "",
        [FromQuery] ushort pageSize = 10,
        [FromQuery] ushort currentPage = 1
        )
    {
        IResponse resp = _service.Get(new(currentPage, pageSize, name, email));
        return StatusCode(resp.Type, resp.Value);
    }

    // GET api/users/5
    [HttpGet("{id}"), Authorize]
    public ActionResult Get(Guid id)
    {
        var resp = _service.GetById(new() { ID = id });
        return StatusCode(resp.Type, resp.Value);
    }

    // POST api/users
    [HttpPost]
    public ActionResult Post([FromBody] CreateUserDTO createUserDTO)
    {

        var resp = _service.Create(createUserDTO);
        return StatusCode(resp.Type, resp.Value);
    }

    // PUT api/users/5
    [HttpPut("{id}"), Authorize]
    public ActionResult Put(Guid id, [FromBody] UpdateUserDTO updateUserDTO)
    {
        updateUserDTO.CurrentUserID = JWT.GetCurrentUserID(HttpContext);
        updateUserDTO.ID = id;

        var resp = _service.Update(updateUserDTO);
        return StatusCode(resp.Type, resp.Value);
    }

    // DELETE api/users/5
    [HttpDelete("{id}"), Authorize]
    public ActionResult Delete(Guid id)
    {
        var resp = _service.Delete(new()
        {
            ID = id,
            CurrentUserID = JWT.GetCurrentUserID(HttpContext)
        });
        return StatusCode(resp.Type, resp.Value);
    }

    // POST api/users/auth
    [HttpPost("auth")]
    public ActionResult Login(LoginUserDTO loginUserDTO)
    {
        var resp = _service.Login(loginUserDTO);
        if (resp.Value is not ResponseUserDTO userDTO) return StatusCode(resp.Type, resp.Value);

        string jwt = _jwt.GenerateToken(userDTO.ID);
        if (jwt == string.Empty) return StatusCode(401, new { Message = "Couldn't create your authentication token" });
        return StatusCode(resp.Type, new
        {
            userDTO.ID,
            userDTO.Email,
            userDTO.Name,
            jwt

        });

    }
    // PUT api/users/5/changePassword
    [HttpPut("{id}/changePassword"), Authorize]
    public ActionResult ChangePassword(Guid id, ChangePasswordDTO changePasswordDTO)
    {
        changePasswordDTO.ID = id;
        changePasswordDTO.CurrentUserID = JWT.GetCurrentUserID(HttpContext);
        var response = _service.ChangePassword(changePasswordDTO);
        return StatusCode(response.Type, response.Value);

    }


}

