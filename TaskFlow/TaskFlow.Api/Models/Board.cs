namespace TaskFlow.Api.Models
{
    public class Board
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<List> Lists { get; set; }
    }
}
