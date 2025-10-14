using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Services;

namespace TaskFlow.Api.Controllers
{
    [Authorize]
    [Route("api/boards/{boardId}/members")]
    [ApiController]
    public class BoardMembershipsController : ControllerBase
    {
        private readonly IBoardService _boardService;

        public BoardMembershipsController(IBoardService boardService)
        {
            _boardService = boardService;
        }

        [HttpPost("invite")]
        public async Task<IActionResult> InviteUser(int boardId, [FromBody] InviteUserDto inviteDto)
        {
            var result = await _boardService.InviteUserToBoardAsync(boardId, inviteDto.Username);

            if (result is ForbidResult)
            {
                return Forbid("Only the board owner can invite members.");
            }
            if (result is NotFoundObjectResult notFound)
            {
                return NotFound(notFound.Value);
            }
            if (result is BadRequestObjectResult badRequest)
            {
                return BadRequest(badRequest.Value);
            }

            return Ok("User successfully invited to the board.");
        }
    }
}