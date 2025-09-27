using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services
{
    public interface IListService
    {
        Task<IEnumerable<ListDto>> GetListsByBoardIdAsync(int boardId);
        Task<List?> GetListByIdAsync(int listId);
        Task<List> CreateListAsync(int boardId, CreateListDto createListDto);
        Task<bool> UpdateListAsync(int listId, UpdateListDto updateListDto);
        Task<bool> DeleteListAsync(int listId);
    }
}
