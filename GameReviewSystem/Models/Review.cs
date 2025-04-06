namespace GameReviewSystem.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;

        // Foreign Keys
        public int GameId { get; set; }
        public Game? Game { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } // navigation back to user
    }
}
