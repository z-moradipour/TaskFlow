using System.ComponentModel.DataAnnotations;

namespace TaskFlow.Api.DTOs
{
    public class InviteUserDto
    {
        [Required]
        public string Username { get; set; }
    }
}
