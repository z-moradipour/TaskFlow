using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Services;

namespace TaskFlow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListsController : ControllerBase
    {
        private readonly IListService _listService;

        public ListsController(IListService listService)
        {
            _listService = listService;
        }

        [HttpGet("board/{boardId}")]
        public async Task<ActionResult<IEnumerable<ListDto>>> GetListsByBoard(int boardId)
        {
            var lists = await _listService.GetListsByBoardIdAsync(boardId);
            return Ok(lists);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetList(int id)
        {
            var list = await _listService.GetListByIdAsync(id);
            if (list == null)
            {
                return NotFound();
            }
            return Ok(list);
        }

        [HttpPost("board/{boardId}")]
        public async Task<IActionResult> CreateList(int boardId, CreateListDto createListDto)
        {
            var newList = await _listService.CreateListAsync(boardId, createListDto);
            return CreatedAtAction(nameof(GetList), new { id = newList.Id }, newList);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateList(int id, UpdateListDto updateListDto)
        {
            var success = await _listService.UpdateListAsync(id, updateListDto);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteList(int id)
        {
            var success = await _listService.DeleteListAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}