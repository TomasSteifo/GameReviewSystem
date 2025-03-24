namespace GameReviewSystem.Models
{
    public class Game
    {
        public int GameId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Platform { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;

        // An enum or string for Status can both work:
        // public StatusEnum Status { get; set; } 
        public string Status { get; set; } = "Backlog";

        // Navigation property
        public ICollection<Review>? Reviews { get; set; }
    }
}
