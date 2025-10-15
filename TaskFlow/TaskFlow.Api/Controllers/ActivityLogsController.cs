using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Services;

namespace TaskFlow.Api.Controllers
{
    [Authorize]
    [Route("api/boards/{boardId}/activities")]
    [ApiController]
    public class ActivityLogsController : ControllerBase
    {
        private readonly IAuditService _auditService;
        private readonly IBoardService _boardService;

        public ActivityLogsController(IAuditService auditService, IBoardService boardService)
        {
            _auditService = auditService;
            _boardService = boardService;
        }

        // GET: api/boards/5/activities
        [HttpGet]
        public async Task<IActionResult> GetBoardActivities(int boardId)
        {
            var boardResult = await _boardService.GetBoardByIdAsync(boardId);
            if (boardResult.Result is ForbidResult || boardResult.Result is UnauthorizedResult)
            {
                return Forbid();
            }

            var activities = await _auditService.GetActivitiesForBoardAsync(boardId);
            return Ok(activities);
        }
    }
}