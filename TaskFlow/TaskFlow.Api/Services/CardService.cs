using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services
{
    public class CardService : ICardService
    {
        private readonly ApplicationDbContext _context;

        public CardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CardDto>> GetCardsByListIdAsync(int listId)
        {
            return await _context.Cards
                .Where(c => c.ListId == listId)
                .OrderBy(c => c.Position)
                .Select(c => new CardDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    Position = c.Position
                })
                .ToListAsync();
        }

        public async Task<Card?> GetCardByIdAsync(int cardId)
        {
            return await _context.Cards.FindAsync(cardId);
        }

        public async Task<Card> CreateCardAsync(int listId, CreateCardDto createCardDto)
        {
            var position = await _context.Cards.CountAsync(c => c.ListId == listId);
            var card = new Card
            {
                Title = createCardDto.Title,
                ListId = listId,
                Position = position
            };
            _context.Cards.Add(card);
            await _context.SaveChangesAsync();
            return card;
        }

        public async Task<bool> UpdateCardAsync(int cardId, UpdateCardDto updateCardDto)
        {
            var card = await _context.Cards.FindAsync(cardId);
            if (card == null)
            {
                return false;
            }
            card.Title = updateCardDto.Title;
            card.Description = updateCardDto.Description;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCardAsync(int cardId)
        {
            var card = await _context.Cards.FindAsync(cardId);
            if (card == null)
            {
                return false;
            }
            _context.Cards.Remove(card);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MoveCardAsync(int cardId, int newListId)
        {
            var card = await _context.Cards.FindAsync(cardId);
            var newList = await _context.Lists.FindAsync(newListId);

            if (card == null || newList == null)
            {
                return false;
            }

            card.ListId = newListId;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}