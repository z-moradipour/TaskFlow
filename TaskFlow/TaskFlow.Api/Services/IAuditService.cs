using TaskFlow.Api.DTOs;

namespace TaskFlow.Api.Services
{
    public interface IAuditService
    {
        Task LogActivityAsync(int boardId, string userId, string description);
        Task<IEnumerable<ActivityLogDto>> GetActivitiesForBoardAsync(int boardId);
    }
}
