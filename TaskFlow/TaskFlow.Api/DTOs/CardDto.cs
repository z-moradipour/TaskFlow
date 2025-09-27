namespace TaskFlow.Api.DTOs
{
    public class CardDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int Position { get; set; }
    }
}