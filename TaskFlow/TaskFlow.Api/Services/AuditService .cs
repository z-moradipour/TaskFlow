using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Data;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services
{
    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;

        public AuditService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogActivityAsync(int boardId, string userId, string description)
        {
            if (string.IsNullOrEmpty(userId) || boardId <= 0)
            {
                return;
            }

            var log = new ActivityLog
            {
                BoardId = boardId,
                UserId = userId,
                Description = description,
                Timestamp = DateTime.UtcNow
            };

            _context.ActivityLogs.Add(log);
            await Task.CompletedTask;
        }
        public async Task<IEnumerable<ActivityLogDto>> GetActivitiesForBoardAsync(int boardId)
        {
            return await _context.ActivityLogs
                .Where(log => log.BoardId == boardId)
                .OrderByDescending(log => log.Timestamp) // Show the newest activities first
                .Include(log => log.User) // Include user data to get the username
                .Select(log => new ActivityLogDto
                {
                    Id = log.Id,
                    Description = log.Description,
                    Timestamp = log.Timestamp,
                    Username = log.User.UserName
                })
                .Take(20) // To avoid sending too much data, limit it to the last 20 activities
                .ToListAsync();
        }
    }
}