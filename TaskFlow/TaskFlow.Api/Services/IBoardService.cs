using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services
{
    public interface IBoardService
    {
        Task<IEnumerable<Board>> GetAllBoardsAsync();
        Task<ActionResult<BoardDto>> GetBoardByIdAsync(int id);
        Task<ActionResult<Board>> CreateBoardAsync(CreateBoardDto createBoardDto);
        Task<ActionResult> InviteUserToBoardAsync(int boardId, string usernameToInvite);
    }
}
