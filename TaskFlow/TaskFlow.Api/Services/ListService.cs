using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services
{
    public class ListService : IListService
    {
        private readonly ApplicationDbContext _context;

        public ListService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ListDto>> GetListsByBoardIdAsync(int boardId)
        {
            return await _context.Lists
                .Where(l => l.BoardId == boardId)
                .OrderBy(l => l.Position)
                .Select(l => new ListDto
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
                })
                .ToListAsync();
        }

        public async Task<List?> GetListByIdAsync(int listId)
        {
            return await _context.Lists.FindAsync(listId);
        }

        public async Task<List> CreateListAsync(int boardId, CreateListDto createListDto)
        {
            var position = await _context.Lists.CountAsync(l => l.BoardId == boardId);
            var list = new List
            {
                Title = createListDto.Title,
                BoardId = boardId,
                Position = position
            };
            _context.Lists.Add(list);
            await _context.SaveChangesAsync();
            return list;
        }

        public async Task<bool> UpdateListAsync(int listId, UpdateListDto updateListDto)
        {
            var list = await _context.Lists.FindAsync(listId);
            if (list == null)
            {
                return false;
            }
            list.Title = updateListDto.Title;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteListAsync(int listId)
        {
            var list = await _context.Lists.FindAsync(listId);
            if (list == null)
            {
                return false;
            }
            _context.Lists.Remove(list);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}