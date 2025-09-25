namespace TaskFlow.Api.Models
{
    public class List
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Position { get; set; }
        public int BoardId { get; set; }
        public Board Board { get; set; }

        // One List has many Cards
        public ICollection<Card> Cards { get; set; }
    }
}
