using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Services;

namespace TaskFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly ICardService _cardService;

        public CardsController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [HttpGet("list/{listId}")]
        public async Task<ActionResult<IEnumerable<CardDto>>> GetCardsByList(int listId)
        {
            var cards = await _cardService.GetCardsByListIdAsync(listId);
            return Ok(cards);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCard(int id)
        {
            var card = await _cardService.GetCardByIdAsync(id);
            if (card == null)
            {
                return NotFound();
            }
            return Ok(card);
        }

        [HttpPost("list/{listId}")]
        public async Task<IActionResult> CreateCard(int listId, CreateCardDto createCardDto)
        {
            var newCard = await _cardService.CreateCardAsync(listId, createCardDto);
            return CreatedAtAction(nameof(GetCard), new { id = newCard.Id }, newCard);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCard(int id, UpdateCardDto updateCardDto)
        {
            var success = await _cardService.UpdateCardAsync(id, updateCardDto);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            var success = await _cardService.DeleteCardAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("reorder/{listId}")]
        public async Task<IActionResult> ReorderCards(int listId, [FromBody] ReorderCardsDto reorderDto)
        {
            await _cardService.ReorderCardsAsync(listId, reorderDto.OrderedCardIds);
            return NoContent();
        }
    }
}