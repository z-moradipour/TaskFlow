using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskFlow.Api.Data;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services
{
    public class BoardService : IBoardService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BoardService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private string CurrentUserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public async Task<IEnumerable<Board>> GetAllBoardsAsync()
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return [];
            }

            // This is the key:
            // 1. Go to the BoardMemberships table.
            // 2. Find all entries for the current user.
            // 3. Select the Board associated with each of those entries.
            return await _context.BoardMemberships
                .Where(bm => bm.UserId == CurrentUserId)
                .Select(bm => bm.Board)
                .ToListAsync();
        }

        public async Task<ActionResult<BoardDto>> GetBoardByIdAsync(int id)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return new UnauthorizedResult();
            }

            var hasAccess = await _context.BoardMemberships
                .AnyAsync(bm => bm.BoardId == id && bm.UserId == CurrentUserId);

            if (!hasAccess)
            {
                return new ForbidResult();
            }

            var boardDto = await _context.Boards
                .Where(b => b.Id == id)
                .Select(b => new BoardDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Lists = b.Lists.OrderBy(l => l.Position).Select(l => new ListDto
                    {
                        Id = l.Id,
                        Title = l.Title,
                        Position = l.Position,
                        Cards = l.Cards.OrderBy(c => c.Position).Select(c => new CardDto
                        {
                            Id = c.Id,
                            Title = c.Title,
                            Description = c.Description,
                            Position = c.Position
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (boardDto == null)
            {
                return new NotFoundResult();
            }

            return boardDto;
        }

        public async Task<ActionResult<Board>> CreateBoardAsync(CreateBoardDto createBoardDto)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return new UnauthorizedResult();
            }

            var board = new Board
            {
                Title = createBoardDto.Title
            };

            // Create a membership for the current user as the Owner
            var membership = new BoardMembership
            {
                UserId = CurrentUserId,
                Board = board,
                Role = BoardRole.Owner
            };

            _context.Boards.Add(board);
            _context.BoardMemberships.Add(membership);

            await _context.SaveChangesAsync();
            return new CreatedAtActionResult("GetBoard", "Boards", new { id = board.Id }, board);
        }
    }
}