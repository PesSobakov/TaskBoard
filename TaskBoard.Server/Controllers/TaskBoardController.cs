using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskBoard.Server.Models.DTOs;
using TaskBoard.Server.Models.TaskBoardDatabase;
using TaskBoard.Server.Services;
using AutoMapper;
using TaskBoard.Server.Models.DTOs.TaskBoard;
using System.Net;
using System.IO;

namespace TaskBoard.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("OpenCORSPolicy")]
    public class TaskBoardController : ControllerBase
    {
        public readonly ITaskBoardDatabaseService _taskBoardDatabaseService;
        private readonly IMapper _mapper;
        public TaskBoardController(ITaskBoardDatabaseService taskBoardDatabaseService, IMapper mapper)
        {
            _taskBoardDatabaseService = taskBoardDatabaseService;
            _mapper = mapper;
        }

        [HttpPost]
        [ActionName("createboard")]
        public async Task<IActionResult> CreateBoard([FromBody] CreateBoardDto createBoardDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.CreateBoard(login, createBoardDto);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet]
        [ActionName("getboards")]
        public async Task<IActionResult> GetBoards()
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.GetBoards(login);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    List<Board> boards = response.Data!;
                    List<GetBoardsDto> boardsDto = boards.Select(_mapper.Map<GetBoardsDto>).ToList();
                    return Ok(boardsDto);
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{id}")]
        [ActionName("getboard")]
        public async Task<IActionResult> GetBoard(int id)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.GetBoard(login, id);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    Board board = response.Data!;
                    BoardDto boardDto = _mapper.Map<BoardDto>(board);
                    return Ok(boardDto);
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPatch("{id}")]
        [ActionName("editboard")]
        public async Task<IActionResult> EditBoard(int id, [FromBody] EditBoardDto editBoardDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.EditBoard(login, id, editBoardDto);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        [ActionName("deleteboard")]
        public async Task<IActionResult> DeleteBoard(int id)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.DeleteBoard(login, id);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        //list create rename change_order delete

        [HttpPost]
        [ActionName("createlist")]
        public async Task<IActionResult> CreateList([FromBody] CreateListDto createListDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.CreateList(login, createListDto);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPatch("{id}")]
        [ActionName("editlist")]
        public async Task<IActionResult> EditList(int id, [FromBody] EditListDto editListDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.EditList(login, id, editListDto);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPatch("{id}")]
        [ActionName("changelistorder")]
        public async Task<IActionResult> ChangeListOrder(int id, [FromBody] ChangeListOrderDto changeListOrderDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.ChangeListOrder(login, id, changeListOrderDto);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        [ActionName("deletelist")]
        public async Task<IActionResult> DeleteList(int id)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.DeleteList(login, id);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        //card create get_card change_props change_order delete

        [HttpPost]
        [ActionName("createcard")]
        public async Task<IActionResult> CreateCard([FromBody] CreateCardDto createCardDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.CreateCard(login, createCardDto);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("{id}")]
        [ActionName("getcard")]
        public async Task<IActionResult> GetCard(int id)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.GetCard(login, id);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    Card card = response.Data!;
                    CardDto cardDto = _mapper.Map<CardDto>(card);
                    return Ok(cardDto);
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPatch("{id}")]
        [ActionName("editcard")]
        public async Task<IActionResult> EditCard(int id, [FromBody] EditCardDto editCardDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.EditCard(login, id, editCardDto);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPatch("{id}")]
        [ActionName("movecard")]
        public async Task<IActionResult> MoveCard(int id, [FromBody] MoveCardDto moveCardDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.MoveCard(login, id, moveCardDto);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPatch("{id}")]
        [ActionName("changecardorder")]
        public async Task<IActionResult> ChangeCardOrder(int id, [FromBody] ChangeCardOrderDto changeCardOrderDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.ChangeCardOrder(login, id, changeCardOrderDto);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        [ActionName("deletecard")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.DeleteCard(login, id);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        //comment add update delete

        [HttpPost]
        [ActionName("createcomment")]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto createCommentDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.CreateComment(login, createCommentDto);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPatch("{id}")]
        [ActionName("editcomment")]
        public async Task<IActionResult> EditComment(int id, [FromBody] EditCommentDto editCommentDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.EditComment(login, id, editCommentDto);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        [ActionName("deletecomment")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.DeleteComment(login, id);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        //privatness setPrivate setPublic

        [HttpPatch("{id}")]
        [ActionName("setboardprivate")]
        public async Task<IActionResult> SetBoardPrivate(int id)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.SetBoardPrivate(login, id);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPatch("{id}")]
        [ActionName("setboardpublic")]
        public async Task<IActionResult> SetBoardPublic(int id)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.SetBoardPublic(login, id);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        //access giveAccess removeAccess

        [HttpPost("{id}")]
        [ActionName("giveaccess")]
        public async Task<IActionResult> GiveAccess(int id, GiveAccessDto giveAccessDto )
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.GiveAccess(login, id, giveAccessDto);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete("{id}")]
        [ActionName("removeaccess")]
        public async Task<IActionResult> RemoveAccess(int id, RemoveAccessDto removeAccessDto)
        {
            string? login = HttpContext.User.Claims.Where(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Select(x => x.Value).FirstOrDefault();
            if (login == null) { return Unauthorized(); }

            var response = await _taskBoardDatabaseService.RemoveAccess(login, id, removeAccessDto);
            switch (response.Status)
            {
                case ResponseStatus.Ok:
                    return Ok();
                case ResponseStatus.Unauthorized:
                    return Unauthorized();
                case ResponseStatus.Forbidden:
                    return Forbid();
                case ResponseStatus.BadRequest:
                    return BadRequest();
                default:
                    return StatusCode((int)HttpStatusCode.InternalServerError);
            }
        }





    }
}
