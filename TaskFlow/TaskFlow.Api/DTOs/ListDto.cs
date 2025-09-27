namespace TaskFlow.Api.DTOs
{
    public class ListDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Position { get; set; }
        public List<CardDto> Cards { get; set; } = [];
    }
}