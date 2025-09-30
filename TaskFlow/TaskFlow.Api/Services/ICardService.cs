using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services
{
    public interface ICardService
    {
        Task<IEnumerable<CardDto>> GetCardsByListIdAsync(int listId);
        Task<ActionResult<CardDto>> GetCardByIdAsync(int cardId);
        Task<Card> CreateCardAsync(int listId, CreateCardDto createCardDto);
        Task<bool> UpdateCardAsync(int cardId, UpdateCardDto updateCardDto);
        Task<bool> DeleteCardAsync(int cardId);
        Task ReorderCardsAsync(int listId, List<int> orderedCardIds);
    }
}
