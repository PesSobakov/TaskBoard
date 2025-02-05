using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskBoard.Server.Models.DTOs;
using TaskBoard.Server.Models.TaskBoardDatabase;
using TaskBoard.Server.Services;
using System.Net;

namespace TaskBoard.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("OpenCORSPolicy")]
    public class AuthController : ControllerBase
    {
        public readonly ITaskBoardDatabaseService _taskBoardDatabaseService;
        public AuthController(ITaskBoardDatabaseService taskBoardDatabaseService)
        {
            _taskBoardDatabaseService = taskBoardDatabaseService;
        }

        [HttpGet]
        [ActionName("getuser")]
        public async Task<IActionResult> GetUser()
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Ok(new StringDto() { value = null });
            }

            var response = await _taskBoardDatabaseService.GetUser(login);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    string user = response.Data!.Login;
                    return Ok(new StringDto() { value=user} );
                case ResponseStatus.BadRequest:
                    return Ok(new StringDto() { value = null });
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }


        [HttpPost]
        [ActionName("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var response = await _taskBoardDatabaseService.Register(registerDto);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    User user = response.Data!;
                    var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login) };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    return Ok();
                case ResponseStatus.UserExists:
                    return BadRequest("This login already used");
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        [ActionName("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _taskBoardDatabaseService.Login(loginDto);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    User user = response.Data!;
                    var claims = new List<Claim> { new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login) };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    return Ok();
                case ResponseStatus.UserNotExists:
                    return BadRequest("This login does not exists");
                case ResponseStatus.WrongPassword:
                    return BadRequest("Wrong password");
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [ActionName("logout")]
        public async Task<OkResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }

        [HttpGet]
        [ActionName("deleteaccount")]
        public async Task<IActionResult> DeleteAccount()
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null)
            {
                return Unauthorized();
            }

            var response = await _taskBoardDatabaseService.DeleteAccount(login);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return Ok();
                case ResponseStatus.UserNotExists:
                    return Unauthorized();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [ActionName("seed")]
        public async Task<IActionResult> Seed()
        {
            var response = await _taskBoardDatabaseService.Seed();
            return (object)response.Status switch
            {
                ResponseStatus.Ok => Ok(),
                _ => StatusCode((int)HttpStatusCode.InternalServerError),
            };
        }

    }
}
