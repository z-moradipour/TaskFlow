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

        [HttpGet]
        public async Task<IActionResult> GetMembers(int boardId)
        {
            var result = await _boardService.GetBoardMembersAsync(boardId);
            if (result.Result is ForbidResult) return Forbid();
            return Ok(result.Value);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemoveMember(int boardId, string userId)
        {
            var result = await _boardService.RemoveUserFromBoardAsync(boardId, userId);
            if (result is ForbidResult) return Forbid("Only the board owner can remove members.");
            if (result is NotFoundResult) return NotFound("Member not found on this board.");
            if (result is BadRequestObjectResult badRequest) return BadRequest(badRequest.Value);

            return NoContent();
        }
    }
}