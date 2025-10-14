using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskFlow.Api.Data;
using TaskFlow.Api.DTOs;
using TaskFlow.Api.Models;

namespace TaskFlow.Api.Services
{
    public class BoardService : IBoardService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BoardService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private string CurrentUserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public async Task<IEnumerable<Board>> GetAllBoardsAsync()
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return [];
            }

            // This is the key:
            // 1. Go to the BoardMemberships table.
            // 2. Find all entries for the current user.
            // 3. Select the Board associated with each of those entries.
            return await _context.BoardMemberships
                .Where(bm => bm.UserId == CurrentUserId)
                .Select(bm => bm.Board)
                .ToListAsync();
        }

        public async Task<ActionResult<BoardDto>> GetBoardByIdAsync(int id)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return new UnauthorizedResult();
            }

            var membership = await _context.BoardMemberships
                .FirstOrDefaultAsync(bm => bm.BoardId == id && bm.UserId == CurrentUserId);

            if (membership == null)
            {
                return new ForbidResult(); // User is not a member, deny access.
            }

            var boardDto = await _context.Boards
                .Where(b => b.Id == id)
                .Select(b => new BoardDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Lists = b.Lists.OrderBy(l => l.Position).Select(l => new ListDto
                    {
                        Id = l.Id,
                        Title = l.Title,
                        Position = l.Position,
                        Cards = l.Cards.OrderBy(c => c.Position).Select(c => new CardDto
                        {
                            Id = c.Id,
                            Title = c.Title,
                            Description = c.Description,
                            Position = c.Position
                        }).ToList()
                    }).ToList(),
                    CurrentUserRole = membership.Role.ToString()
                })
                .FirstOrDefaultAsync();

            if (boardDto == null)
            {
                return new NotFoundResult();
            }

            return boardDto;
        }

        public async Task<ActionResult<Board>> CreateBoardAsync(CreateBoardDto createBoardDto)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
            {
                return new UnauthorizedResult();
            }

            var board = new Board
            {
                Title = createBoardDto.Title
            };

            // Create a membership for the current user as the Owner
            var membership = new BoardMembership
            {
                UserId = CurrentUserId,
                Board = board,
                Role = BoardRole.Owner
            };

            _context.Boards.Add(board);
            _context.BoardMemberships.Add(membership);

            await _context.SaveChangesAsync();
            return new CreatedAtActionResult("GetBoard", "Boards", new { id = board.Id }, board);
        }

        public async Task<ActionResult> InviteUserToBoardAsync(int boardId, string usernameToInvite)
        {
            var currentUserMembership = await _context.BoardMemberships
                .FirstOrDefaultAsync(bm => bm.BoardId == boardId && bm.UserId == CurrentUserId);

            if (currentUserMembership == null || currentUserMembership.Role != BoardRole.Owner)
            {
                // Only the owner can invite others.
                return new ForbidResult();
            }

            var userToInvite = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == usernameToInvite.ToLower());

            if (userToInvite == null)
            {
                return new NotFoundObjectResult($"User '{usernameToInvite}' not found.");
            }

            var isAlreadyMember = await _context.BoardMemberships
                .AnyAsync(bm => bm.BoardId == boardId && bm.UserId == userToInvite.Id);

            if (isAlreadyMember)
            {
                return new BadRequestObjectResult($"User '{usernameToInvite}' is already a member of this board.");
            }

            var newMembership = new BoardMembership
            {
                BoardId = boardId,
                UserId = userToInvite.Id,
                Role = BoardRole.Member // Invited users are members by default.
            };

            _context.BoardMemberships.Add(newMembership);
            await _context.SaveChangesAsync();

            return new OkResult();
        }

        public async Task<ActionResult<IEnumerable<BoardMemberDto>>> GetBoardMembersAsync(int boardId)
        {
            var hasAccess = await _context.BoardMemberships
                .AnyAsync(bm => bm.BoardId == boardId && bm.UserId == CurrentUserId);

            if (!hasAccess)
            {
                return new ForbidResult(); // Deny access if not a member.
            }

            return await _context.BoardMemberships
                .Where(bm => bm.BoardId == boardId)
                .Include(bm => bm.User)
                .Select(bm => new BoardMemberDto
                {
                    UserId = bm.UserId,
                    Username = bm.User.UserName,
                    Role = bm.Role.ToString()
                })
                .ToListAsync();
        }

        public async Task<ActionResult> RemoveUserFromBoardAsync(int boardId, string userIdToRemove)
        {
            // Only owners can remove members.
            var currentUserMembership = await _context.BoardMemberships
                .FirstOrDefaultAsync(bm => bm.BoardId == boardId && bm.UserId == CurrentUserId);

            if (currentUserMembership == null || currentUserMembership.Role != BoardRole.Owner)
            {
                return new ForbidResult();
            }

            var membershipToRemove = await _context.BoardMemberships
                .FirstOrDefaultAsync(bm => bm.BoardId == boardId && bm.UserId == userIdToRemove);

            if (membershipToRemove == null)
            {
                return new NotFoundResult();
            }

            // Prevent the owner from removing themselves.
            if (membershipToRemove.Role == BoardRole.Owner)
            {
                return new BadRequestObjectResult("The owner of a board cannot be removed.");
            }

            _context.BoardMemberships.Remove(membershipToRemove);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}