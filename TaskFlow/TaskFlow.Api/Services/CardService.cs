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
        public async Task ReorderCardsAsync(int listId, List<int> orderedCardIds)
        {
            // Fetch only the cards that are relevant to this operation.
            var cardsToUpdate = await _context.Cards
                .Where(c => c.ListId == listId || orderedCardIds.Contains(c.Id))
                .ToListAsync();

            // Loop through the cards we found and update them.
            foreach (var card in cardsToUpdate)
            {
                int newPosition = orderedCardIds.IndexOf(card.Id);

                if (newPosition == -1) // Card is no longer in this list
                {
                    if (card.ListId == listId)
                    {
                        // This case is handled by the API call for the other list
                    }
                }
                else // Card is in this list
                {
                    card.ListId = listId;
                    card.Position = newPosition;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}