using GameReviewSystem.Models;


namespace GameReviewSystem.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;

        // If this is a foreign key to the Game:
        public int GameId { get; set; }
        public Game Game { get; set; }  // Navigation property

        // If this is a foreign key to the User:
        public int UserId { get; set; }
        public User User { get; set; }  // Navigation property
    }
}



