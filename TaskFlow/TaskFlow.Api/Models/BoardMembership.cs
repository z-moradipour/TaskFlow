using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.Models
{
    public enum BoardRole
    {
        Owner,
        Member
    }

    public class BoardMembership
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int BoardId { get; set; }
        public Board Board { get; set; }

        public BoardRole Role { get; set; } = BoardRole.Member;
    }
}