namespace TaskFlow.Api.DTOs
{
    public class ActivityLogDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string Username { get; set; }
    }
}