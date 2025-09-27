namespace TaskFlow.Api.DTOs
{
    public class BoardDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<ListDto> Lists { get; set; } = [];
    }
}