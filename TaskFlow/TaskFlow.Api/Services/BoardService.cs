using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services
{
    public class BoardService : IBoardService
    {
        private readonly ApplicationDbContext _context;

        public BoardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Board>> GetAllBoardsAsync()
        {
            return await _context.Boards.ToListAsync();
        }

        public async Task<ActionResult<BoardDto>> GetBoardByIdAsync(int id)
        {
            var board = await _context.Boards
                .Include(b => b.Lists)
                .ThenInclude(l => l.Cards)
                .Where(b => b.Id == id)
                .Select(b => new BoardDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Lists = b.Lists.Select(l => new ListDto
                    {
                        Id = l.Id,
                        Title = l.Title,
                        Position = l.Position,
                        Cards = l.Cards.Select(c => new CardDto
                        {
                            Id = c.Id,
                            Title = c.Title,
                            Description = c.Description,
                            Position = c.Position
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (board == null)
            {
                return new NotFoundResult();
            }

            return board;
        }

        public async Task<ActionResult<Board>> CreateBoardAsync(CreateBoardDto createBoardDto)
        {
            var board = new Board
            {
                Title = createBoardDto.Title
            };

            _context.Boards.Add(board);
            await _context.SaveChangesAsync();

            return new CreatedAtActionResult("GetBoard", "Boards", new { id = board.Id }, board);
        }
    }
}
