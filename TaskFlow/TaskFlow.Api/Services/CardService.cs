using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskFlow.Api.Data;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services
{
    public class CardService : ICardService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditService _auditService;
        private readonly UserManager<User> _userManager;

        public CardService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IAuditService auditService,
            UserManager<User> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _auditService = auditService;
            _userManager = userManager;
        }
        private string CurrentUserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

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

        public async Task<ActionResult<CardDto>> GetCardByIdAsync(int cardId)
        {
            var card = await _context.Cards.FindAsync(cardId);

            if (card == null)
            {
                return new NotFoundResult();
            }

            var cardDto = new CardDto
            {
                Id = card.Id,
                Title = card.Title,
                Description = card.Description,
                Position = card.Position
            };

            return cardDto;
        }

        public async Task<Card> CreateCardAsync(int listId, CreateCardDto createCardDto)
        {
            var listInfo = await _context.Lists
                .AsNoTracking()
                .Where(l => l.Id == listId)
                .Select(l => new { l.Id, l.Title, l.BoardId })
                .FirstOrDefaultAsync();

            if (listInfo == null)
                return null;

            var position = await _context.Cards.CountAsync(c => c.ListId == listId);
            var card = new Card
            {
                Title = createCardDto.Title,
                ListId = listId,
                Position = position
            };
            _context.Cards.Add(card);

            var user = await _userManager.FindByIdAsync(CurrentUserId);
            var description = $"{user?.UserName} created card '{card.Title}' in list '{listInfo.Title}'";
            await _auditService.LogActivityAsync(listInfo.BoardId, CurrentUserId, description);

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
        //public async Task ReorderCardsAsync(int listId, List<int> orderedCardIds)
        //{
        //    var cardsInNewOrder = await _context.Cards
        //        .Where(c => orderedCardIds.Contains(c.Id))
        //        .ToListAsync();

        //    for (int i = 0; i < orderedCardIds.Count; i++)
        //    {
        //        int cardId = orderedCardIds[i];
        //        var card = cardsInNewOrder.FirstOrDefault(c => c.Id == cardId);

        //        if (card != null)
        //        {
        //            card.ListId = listId;
        //            card.Position = i;
        //        }
        //    }

        //    await _context.SaveChangesAsync();
        //}
        public async Task ReorderCardsAsync(int listId, List<int> orderedCardIds)
        {
            var cards = await _context.Cards
                .Where(c => orderedCardIds.Contains(c.Id))
                .ToListAsync();

            var cardDict = cards.ToDictionary(c => c.Id);

            if (!cards.Any())
                return;

            var allListIds = cards.Select(c => c.ListId).Append(listId).Distinct().ToList();
            var lists = await _context.Lists
                .AsNoTracking()
                .Where(l => allListIds.Contains(l.Id))
                .ToDictionaryAsync(l => l.Id, l => l.Title);

            var boardId = await _context.Lists
                .Where(l => l.Id == listId)
                .Select(l => l.BoardId)
                .FirstOrDefaultAsync();

            var user = await _userManager.FindByIdAsync(CurrentUserId);

            for (int i = 0; i < orderedCardIds.Count; i++)
            {
                var cardId = orderedCardIds[i];
                if (!cardDict.TryGetValue(cardId, out var card))
                    continue;

                if (card.ListId != listId)
                {
                    var oldListId = card.ListId;
                    card.ListId = listId;
                    card.Position = i;

                    await _auditService.LogActivityAsync(
                        boardId,
                        CurrentUserId,
                        $"{user?.UserName} moved card '{card.Title}' from list '{lists[oldListId]}' to list '{lists[listId]}'"
                    );
                }
                else
                {
                    card.Position = i;
                }
            }

            await _context.SaveChangesAsync();
        }

    }
}