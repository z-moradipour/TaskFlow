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
                        // Make sure the cards within each list are ordered
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
